using FluentValidation;

namespace ObservabilityApi.Application.Common;

public sealed record PageRequest(int Page = PageRequest.DefaultPage, int PageSize = PageRequest.DefaultPageSize)
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>Bounds OFFSET scans; deep paging should move to keyset pagination (see README).</summary>
    public const int MaxPage = 1_000;

    public int Skip => (Page - 1) * PageSize;
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public PageRequestValidator()
    {
        RuleFor(x => x.Page).InclusiveBetween(1, PageRequest.MaxPage);
        RuleFor(x => x.PageSize).InclusiveBetween(1, PageRequest.MaxPageSize);
    }
}
