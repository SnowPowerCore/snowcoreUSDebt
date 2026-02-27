using Debt.ConsoleClient.Services;
using Debt.ConsoleClient.Utils;
using Debt.ConsoleClient.Interfaces;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Utils;
using Moq;

namespace Debt.ConsoleClient.Tests;

public class ConsoleNavigationServiceTests
{
    private readonly Mock<IServiceProvider> _provider;
    private readonly KnownScreenOptions _knownScreens;
    private readonly ConsoleNavigationService _navigation;

    public ConsoleNavigationServiceTests()
    {
        _provider = new Mock<IServiceProvider>(MockBehavior.Strict);
        _knownScreens = new KnownScreenOptions();
        _navigation = new ConsoleNavigationService(Options.Create(_knownScreens), _provider.Object);
    }

    [Fact]
    public async Task NavigateToScreenAsync_CallsAppearingAndInit()
    {
        var firstScreen = CreateScreenMock();

        _provider
            .Setup(serviceProvider => serviceProvider.GetService(typeof(IConsoleScreen)))
            .Returns(firstScreen.Object);
        _knownScreens.KnownScreenTypes["first"] = typeof(IConsoleScreen);

        await _navigation.NavigateToScreenAsync("first");

        firstScreen.Verify(screen => screen.OnScreenAppearingAsync(It.IsAny<DictionaryWithDefault<string, object>?>()), Times.Once);
        firstScreen.Verify(screen => screen.InitScreenAsync(), Times.Once);
    }

    [Fact]
    public async Task NavigateBackAsync_WithTwoScreens_DisposesTopAndReinitializesPrevious()
    {
        var firstScreen = CreateScreenMock();

        var secondScreen = CreateScreenMock();
        secondScreen
            .Setup(screen => screen.OnScreenDisappearingAsync())
            .Returns(Task.CompletedTask);

        var secondScreenDisposable = secondScreen.As<IAsyncDisposable>();
        secondScreenDisposable
            .Setup(disposable => disposable.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        _provider
            .SetupSequence(serviceProvider => serviceProvider.GetService(typeof(IConsoleScreen)))
            .Returns(firstScreen.Object)
            .Returns(secondScreen.Object);

        _knownScreens.KnownScreenTypes["first"] = typeof(IConsoleScreen);
        _knownScreens.KnownScreenTypes["second"] = typeof(IConsoleScreen);

        await _navigation.NavigateToScreenAsync("first");
        await _navigation.NavigateToScreenAsync("second");

        await _navigation.NavigateBackAsync();

        secondScreen.Verify(screen => screen.OnScreenDisappearingAsync(), Times.Once);
        secondScreenDisposable.Verify(disposable => disposable.DisposeAsync(), Times.Once);
        firstScreen.Verify(screen => screen.OnScreenAppearingAsync(It.IsAny<DictionaryWithDefault<string, object>?>()), Times.Exactly(2));
        firstScreen.Verify(screen => screen.InitScreenAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task NavigateBackAsync_WithEmptyStack_DoesNothing()
    {
        await _navigation.NavigateBackAsync();
    }

    private static Mock<IConsoleScreen> CreateScreenMock()
    {
        var screen = new Mock<IConsoleScreen>(MockBehavior.Strict);
        screen
            .Setup(current => current.OnScreenAppearingAsync(It.IsAny<DictionaryWithDefault<string, object>?>()))
            .Returns(Task.CompletedTask);
        screen
            .Setup(current => current.InitScreenAsync())
            .Returns(Task.CompletedTask);
        return screen;
    }
}