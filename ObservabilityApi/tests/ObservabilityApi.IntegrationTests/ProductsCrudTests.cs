using System.Net;
using System.Net.Http.Json;
using ObservabilityApi.Application.Common;
using ObservabilityApi.Application.Products;
using ObservabilityApi.IntegrationTests.Infrastructure;

namespace ObservabilityApi.IntegrationTests;

public sealed class ProductsCrudTests(ApiFactory factory)
{
    private readonly HttpClient client = factory.CreateClient();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Create_get_update_delete_round_trip()
    {
        var sku = HttpAssertions.UniqueSku();

        // Create
        var createResponse = await client.PostAsJsonAsync("/api/products", new CreateProductRequest(sku.ToLowerInvariant(), " Widget ", null, 19.99m), Ct);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = (await createResponse.Content.ReadFromJsonAsync<ProductResponse>(Ct))!;
        Assert.Equal(sku, created.Sku); // normalized to upper case
        Assert.Equal("Widget", created.Name);
        Assert.Equal($"/api/products/{created.Id}", createResponse.Headers.Location?.AbsolutePath, ignoreCase: true);

        // Read
        var fetched = await client.GetFromJsonAsync<ProductResponse>($"/api/products/{created.Id}", Ct);
        Assert.Equal(created, fetched);

        // Update with the current version
        var updateResponse = await client.PutAsJsonAsync($"/api/products/{created.Id}",
            new UpdateProductRequest("Widget Pro", "Now with more widget", 24.50m, created.Version), Ct);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = (await updateResponse.Content.ReadFromJsonAsync<ProductResponse>(Ct))!;
        Assert.Equal("Widget Pro", updated.Name);
        Assert.Equal(24.50m, updated.Price);
        Assert.NotEqual(created.Version, updated.Version);

        // Delete, then it is gone
        var deleteResponse = await client.DeleteAsync($"/api/products/{created.Id}", Ct);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client.GetAsync($"/api/products/{created.Id}", Ct);
        await afterDelete.AssertProblemAsync(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Duplicate_sku_is_rejected_with_409()
    {
        var existing = await client.CreateProductAsync();

        var response = await client.PostAsJsonAsync("/api/products",
            new CreateProductRequest(existing.Sku.ToLowerInvariant(), "Another", null, 1m), Ct);

        var problem = await response.AssertProblemAsync(HttpStatusCode.Conflict);
        Assert.Contains(existing.Sku, problem.GetProperty("detail").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Concurrent_creates_with_same_sku_produce_exactly_one_product()
    {
        var sku = HttpAssertions.UniqueSku();
        var request = new CreateProductRequest(sku, "Race", null, 1m);

        var responses = await Task.WhenAll(Enumerable.Range(0, 5)
            .Select(_ => client.PostAsJsonAsync("/api/products", request, Ct)));

        // The unique index is the source of truth even when the pre-check races.
        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.All(responses.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
    }

    [Fact]
    public async Task Update_with_stale_version_is_rejected_with_409()
    {
        var product = await client.CreateProductAsync();
        var first = await client.PutAsJsonAsync($"/api/products/{product.Id}",
            new UpdateProductRequest("First writer", null, 2m, product.Version), Ct);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        // Second writer still holds the original version.
        var second = await client.PutAsJsonAsync($"/api/products/{product.Id}",
            new UpdateProductRequest("Second writer", null, 3m, product.Version), Ct);

        await second.AssertProblemAsync(HttpStatusCode.Conflict);
        var current = await client.GetFromJsonAsync<ProductResponse>($"/api/products/{product.Id}", Ct);
        Assert.Equal("First writer", current!.Name);
    }

    [Fact]
    public async Task List_is_paged_and_ordered_newest_first()
    {
        for (var i = 0; i < 3; i++)
        {
            await client.CreateProductAsync();
        }

        var page = await client.GetFromJsonAsync<PagedResult<ProductResponse>>("/api/products?page=1&pageSize=2", Ct);

        Assert.NotNull(page);
        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(2, page.Items.Count);
        Assert.True(page.TotalCount >= 3);
        Assert.Equal((int)Math.Ceiling(page.TotalCount / 2.0), page.TotalPages);
        Assert.True(page.Items[0].CreatedAtUtc >= page.Items[1].CreatedAtUtc);
    }

    [Theory]
    [InlineData("/api/products?page=0")]
    [InlineData("/api/products?pageSize=101")]
    [InlineData("/api/products?page=1001")]
    public async Task Paging_is_bounded(string url)
    {
        var response = await client.GetAsync(url, Ct);
        var problem = await response.AssertProblemAsync(HttpStatusCode.BadRequest);
        Assert.True(problem.TryGetProperty("errors", out _));
    }

    [Fact]
    public async Task Update_and_delete_of_unknown_product_return_404()
    {
        var id = Guid.NewGuid();
        var update = await client.PutAsJsonAsync($"/api/products/{id}",
            new UpdateProductRequest("x", null, 1m, Convert.ToBase64String(new byte[8])), Ct);
        await update.AssertProblemAsync(HttpStatusCode.NotFound);

        var delete = await client.DeleteAsync($"/api/products/{id}", Ct);
        await delete.AssertProblemAsync(HttpStatusCode.NotFound);
    }
}
