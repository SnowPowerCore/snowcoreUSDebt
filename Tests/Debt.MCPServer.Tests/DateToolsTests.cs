using Debt.MCPServer.Features.DateTime;
using Debt.MCPServer.Interfaces;
using MaybeResults;
using Moq;

namespace Debt.MCPServer.Tests;

public class DateToolsTests
{
    private readonly Mock<IDateTimeService> _service;
    private readonly DateTools _sut;

    public DateToolsTests()
    {
        _service = new Mock<IDateTimeService>(MockBehavior.Strict);
        _sut = new DateTools(_service.Object);
    }

    [Fact]
    public async Task GetCurrentDateToolAsync_ReturnsFormattedDate_WhenServiceReturnsSome()
    {
        var now = DateTimeOffset.UtcNow;
        var expectedDate = now.ToString("yyyy-MM-dd");

        _service
            .Setup(s => s.GetCurrentDateTimeAsync())
            .ReturnsAsync(Maybe.Create(now));

        var result = await _sut.GetCurrentDateToolAsync();

        Assert.Equal(expectedDate, result);
        _service.Verify(s => s.GetCurrentDateTimeAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCurrentDateToolAsync_ReturnsFallbackError_WhenServiceReturnsNonSome()
    {
        _service
            .Setup(s => s.GetCurrentDateTimeAsync())
            .ReturnsAsync((IMaybe<DateTimeOffset>)null!);

        var result = await _sut.GetCurrentDateToolAsync();

        Assert.Equal(Resources.Resource.GetCurrentDateTimeError, result);
        _service.Verify(s => s.GetCurrentDateTimeAsync(), Times.Once);
    }
}