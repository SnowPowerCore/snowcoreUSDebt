using Debt.MCPServer.Features.DateTime;
using MaybeResults;

namespace Debt.MCPServer.Tests;

public class GetCurrentDateServiceTests
{
    [Fact]
    public async Task GetCurrentDateTimeAsync_ReturnsUtcNowAsSome()
    {
        var sut = new GetCurrentDateService();
        var before = DateTimeOffset.UtcNow;

        var result = await sut.GetCurrentDateTimeAsync();

        var after = DateTimeOffset.UtcNow;
        var some = Assert.IsType<Some<DateTimeOffset>>(result);
        Assert.Equal(TimeSpan.Zero, some.Value.Offset);
        Assert.InRange(some.Value, before.AddSeconds(-1), after.AddSeconds(1));
    }
}