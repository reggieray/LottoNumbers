using FluentAssertions;
using LottoNumbers.Models;
using LottoNumbers.Services;
using LottoNumbers.ViewModels;
using Moq;
using Prism.Navigation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TestStack.BDDfy;
using Xunit;

namespace LottoNumbers.UnitTests.ViewModels
{
    public class MainPageViewModelTests : ViewModelTestsBase<MainPageViewModel>
    {
        private readonly List<LottoGame> LottoGames = new List<LottoGame>()
                {
                    new LottoGame { GameKey = "LOTTO", DisplayName = "Lotto" },
                    new LottoGame { GameKey = "EURO", DisplayName = "Euro" }
                };

        private readonly Dictionary<string, LottoGameSetting> GameSettings = new Dictionary<string, LottoGameSetting>()
                {
                    { "LOTTO", new LottoGameSetting { BallColor = "RED", Count = 6, Min = 1, Max = 60 }},
                    { "EURO", new LottoGameSetting { BallColor = "BLUE", Count = 5, Min = 1, Max = 50, HasBounsNumber = true, BonusBallColor = "GOLD", BonusNumberCount = 2, BonusNumberMin = 1, BonusNumberMax = 13 }},
                };

        private readonly ILottoGameService lottoGameService;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<IRemoteConfigService> mockRemoteConfigService;
        private readonly Mock<INavigationParameters> mockNavigationParameters;

        public MainPageViewModelTests()
        {
            mockNavigationParameters = new Mock<INavigationParameters>();
            mockNavigationService = new Mock<INavigationService>();
            mockRemoteConfigService = new Mock<IRemoteConfigService>();
            mockRemoteConfigService.Setup(x => x.GetAsync<List<LottoGame>>(It.IsAny<string>()))
                .ReturnsAsync(LottoGames);

            mockRemoteConfigService.Setup(x => x.GetAsync<Dictionary<string, LottoGameSetting>>(It.IsAny<string>()))
                .ReturnsAsync(GameSettings);

            lottoGameService = new LottoGameService(mockRemoteConfigService.Object);
            ViewModel = new MainPageViewModel(mockNavigationService.Object, lottoGameService);
            this.SetupOnPropertyChanged();
        }

        [Fact]
        public void UserGeneratesLottoNumbers()
        {
            var gameKey = default(string);

            this.Given(_ => _.AUserNavigatesToTheMainPage())
            .And(_ => _.TheUserIsShownGamesToPick())
            .And(_ => _.TheUserSelectsALottoGame(gameKey))
            .When(_ => _.TheUserClicksGenerateNumbers())
            .Then(_ => _.TheLottoNumbersAreShown())
            .And(_ => _.TheNumbersAreCorrectForGame(gameKey))
            .WithExamples(new ExampleTable("gameKey")
            { 
                "LOTTO",
                "EURO"
            })
            .BDDfy();
        }

        private void AUserNavigatesToTheMainPage()
        {
            MockPropertyChangedEventHandler.Invocations.Clear();
            ViewModel.Initialize(mockNavigationParameters.Object);
        }

        private void TheUserIsShownGamesToPick()
        {
            MockPropertyChangedEventHandler
                .Verify(x => x.Invoke(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(ViewModel.LottoGames))), Times.Once);
        }

        private void TheUserSelectsALottoGame(string gameKey)
        {
            ViewModel.SelectedGame = ViewModel.LottoGames.First(x => x.GameKey == gameKey);
        }

        private void TheUserClicksGenerateNumbers()
        {
            ViewModel.GenerateNumbersCommand.Execute();
        }

        private void TheLottoNumbersAreShown()
        {
            MockPropertyChangedEventHandler
                .Verify(x => x.Invoke(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(ViewModel.LottoNumbers))), Times.Once);
            ViewModel.LottoNumbers.Should().NotBeEmpty();
        }

        private void TheNumbersAreCorrectForGame(string gameKey)
        {
            var gameSetting = GameSettings[gameKey];
            ViewModel.LottoNumbers.Count(x => !x.IsBouns && x.Number >= gameSetting.Min && x.Number < gameSetting.Max)
                .Should().Be(gameSetting.Count);
            ViewModel.LottoNumbers.Count(x => x.IsBouns && x.Number >= gameSetting.BonusNumberMin && x.Number < gameSetting.BonusNumberMax).Should().Be(gameSetting.BonusNumberCount);
        }
    }
}
