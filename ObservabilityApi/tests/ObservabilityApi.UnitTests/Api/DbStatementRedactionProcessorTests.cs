using System.Diagnostics;
using ObservabilityApi.Api.Observability;

namespace ObservabilityApi.UnitTests.Api;

public sealed class DbStatementRedactionProcessorTests
{
    [Fact]
    public void Removes_statement_text_but_keeps_low_cardinality_summary()
    {
        using var activity = new Activity("SELECT catalog.Products");
        activity.SetTag("db.query.text", "SELECT * FROM [catalog].[Products] WHERE [Sku] = @p0");
        activity.SetTag("db.statement", "legacy attribute");
        activity.SetTag("db.query.summary", "SELECT catalog.Products");

        using var processor = new DbStatementRedactionProcessor();
        processor.OnEnd(activity);

        Assert.Null(activity.GetTagItem("db.query.text"));
        Assert.Null(activity.GetTagItem("db.statement"));
        Assert.Equal("SELECT catalog.Products", activity.GetTagItem("db.query.summary"));
    }
}
