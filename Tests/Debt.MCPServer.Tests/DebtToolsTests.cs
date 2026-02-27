using Debt.MCPServer.Features.Debt;
using Debt.MCPServer.Interfaces;
using Debt.MCPServer.Models.Dto;
using MaybeResults;
using Moq;

namespace Debt.MCPServer.Tests;

public class DebtToolsTests
{
    private readonly Mock<IDebtService> _service;
    private readonly DebtTools _sut;

    public DebtToolsTests()
    {
        _service = new Mock<IDebtService>(MockBehavior.Strict);
        _sut = new DebtTools(_service.Object);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsDebtPayloadAndForwardsArguments_WhenServiceReturnsSome()
    {
        GetUsDebtArgs? capturedArgs = null;
        _service
            .Setup(s => s.GetUsDebtAsync(It.IsAny<GetUsDebtArgs?>()))
            .Callback<GetUsDebtArgs?>(args => capturedArgs = args)
            .ReturnsAsync(Maybe.Create("{\"debt\":\"ok\"}"));

        var result = await _sut.GetUsDebtToolAsync("record_date:gte:2026-01-01", "-record_date", 5, 2);

        Assert.Equal("{\"debt\":\"ok\"}", result);
        Assert.NotNull(capturedArgs);
        Assert.Equal("record_date:gte:2026-01-01", capturedArgs!.Filter);
        Assert.Equal("-record_date", capturedArgs.Sort);
        Assert.Equal(5, capturedArgs.PageSize);
        Assert.Equal(2, capturedArgs.PageNumber);
        _service.Verify(s => s.GetUsDebtAsync(It.IsAny<GetUsDebtArgs?>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsFallbackError_WhenServiceReturnsNonSome()
    {
        _service
            .Setup(s => s.GetUsDebtAsync(It.IsAny<GetUsDebtArgs?>()))
            .ReturnsAsync((IMaybe<string>)null!);

        var result = await _sut.GetUsDebtToolAsync();

        Assert.Equal(Resources.Resource.GetUsDebtError, result);
        _service.Verify(s => s.GetUsDebtAsync(It.IsAny<GetUsDebtArgs?>()), Times.Once);
    }
}