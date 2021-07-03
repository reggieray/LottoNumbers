using FluentAssertions;
using LottoNumbers.Constants;
using LottoNumbers.Models;
using LottoNumbers.Services;
using LottoNumbers.ViewModels;
using LottoNumbers.Views;
using Moq;
using System;
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
                    new LottoGame { GameKey = "EURO", DisplayName = "Euro" },
                    new LottoGame { GameKey = "REGIS", DisplayName = "Regis" }
                };

        private readonly Dictionary<string, LottoGameSetting> GameSettings = new Dictionary<string, LottoGameSetting>()
                {
                    { "LOTTO", new LottoGameSetting { BallColor = "RED", Count = 6, Min = 1, Max = 60 }},
                    { "EURO", new LottoGameSetting { BallColor = "BLUE", Count = 5, Min = 1, Max = 50, HasBounsNumber = true, BonusBallColor = "GOLD", BonusNumberCount = 2, BonusNumberMin = 1, BonusNumberMax = 13 }},
                    { "REGIS", new LottoGameSetting { BallColor = "PURPLE", Count = 2, Min = 1, Max = 2 }}
                };

        private readonly ILottoGameService lottoGameService;
        private readonly Mock<IRemoteConfigService> mockRemoteConfigService;
        
        public MainPageViewModelTests()
        {
            mockRemoteConfigService = new Mock<IRemoteConfigService>();

            mockPreferences.Setup(x => x.Get(SettingConstants.PSEUDORANDOM_SEED_KEY, It.IsAny<string>()))
                .Returns(DateTime.Now.ToString(SettingConstants.DATE_FORMAT));

            mockRemoteConfigService.Setup(x => x.GetAsync<List<LottoGame>>(It.IsAny<string>()))
                .ReturnsAsync(LottoGames);

            mockRemoteConfigService.Setup(x => x.GetAsync<Dictionary<string, LottoGameSetting>>(It.IsAny<string>()))
                .ReturnsAsync(GameSettings);

            lottoGameService = new LottoGameService(mockRemoteConfigService.Object, settingsService);
            ViewModel = new MainPageViewModel(mockNavigationService.Object, lottoGameService);
            this.SetupOnPropertyChanged();
        }

        [Fact]
        public void UserGeneratesLottoNumbers()
        {
            var gameKey = default(string);
            var useSeed = default(bool);

            this.Given(_ => _.AUserIsOnTheMainPage())
            .And(_ => _.TheUserIsShownGamesToPick())
            .And(_ => _.TheUserSelectsALottoGame(gameKey))
            .And(_ => _.TheUserHasASeedForRandom(useSeed))
            .When(_ => _.TheUserClicksGenerateNumbers())
            .Then(_ => _.TheLottoNumbersAreShown())
            .And(_ => _.TheGameHeaderIs(gameKey))
            .And(_ => _.TheNumbersAreCorrectForGame(gameKey))
            .WithExamples(new ExampleTable("gameKey", "useSeed")
            {
                { "LOTTO", false },
                { "EURO", false },
                { "REGIS", false },
                { "REGIS", true },
                { "EURO", true }
            })
            .BDDfy();
        }

        [Fact]
        public void UserClicksToViewLuckyCat()
        {
            this.Given(_ => _.AUserIsOnTheMainPage())
            .And(_ => _.TheUserIsShownGamesToPick())
            .And(_ => _.TheUserSelectsALottoGame())
            .And(_ => _.TheUserClicksGenerateNumbers())
            .And(_ => _.TheLottoNumbersAreShown())
            .And(_ => _.TheNumbersAreCorrectForGame())
            .And(_ => _.TheLuckyCatIsNotVisible())
            .When(_ => _.TheUserClicksShowLuckyCat())
            .Then(_ => _.TheLuckyCatIsVisible())
            .BDDfy();
        }

        [Fact]
        public void UserNavigatesToSettings()
        {
            this.Given(_ => _.AUserIsOnTheMainPage())
            .When(_ => _.TheUserClicksToNavigateToSettingsPage())
            .Then(_ => _.TheUserIsNavigatedToSettingsPage())
            .BDDfy();
        }

        private void TheUserHasASeedForRandom(bool useSeed)
        {
           mockPreferences.Setup(x => x.Get(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, It.IsAny<bool>()))
               .Returns(useSeed);
        }

        private void TheGameHeaderIs(string gameKey)
        {
            var expectedGameHeader = $"Your {LottoGames.First(x => x.GameKey == gameKey).DisplayName} numbers are:";
            ViewModel.GameHeader.Should().Be(expectedGameHeader);
        }

        private void TheUserIsNavigatedToSettingsPage()
        {
            mockNavigationService.Verify(c => c.NavigateAsync(nameof(SettingsPage)), Times.Once);
        }

        private void TheUserClicksToNavigateToSettingsPage()
        {
            ViewModel.SettingsNavigationCommand.Execute();
        }

        private void TheLuckyCatIsVisible()
        {
            ViewModel.ShowLuckyCat.Should().BeTrue();
        }

        private void TheUserClicksShowLuckyCat()
        {
            ViewModel.ShowLuckyCatCommand.Execute();
        }

        private void TheLuckyCatIsNotVisible()
        {
            ViewModel.ShowLuckyCat.Should().BeFalse();
        }

        private void AUserIsOnTheMainPage() => this.NavigatesToViewModel();

        private void TheUserIsShownGamesToPick()
        {
            MockPropertyChangedEventHandler
                .Verify(x => x.Invoke(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(ViewModel.LottoGames))), Times.Once);
        }

        private void TheUserSelectsALottoGame()
        {
            TheUserSelectsALottoGame(LottoGames[0].GameKey);
        }

        private void TheUserSelectsALottoGame(string gameKey)
        {
            ViewModel.SelectedGame = ViewModel.LottoGames.First(x => x.GameKey == gameKey);
        }

        private void TheUserClicksGenerateNumbers()
        {
            if (ViewModel.GenerateNumbersCommand.CanExecute())
            {
                ViewModel.GenerateNumbersCommand.Execute();
            }
        }

        private void TheLottoNumbersAreShown()
        {
            MockPropertyChangedEventHandler
                .Verify(x => x.Invoke(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(ViewModel.LottoNumbers))), Times.Once);
            ViewModel.LottoNumbers.Should().NotBeEmpty();
        }

        private void TheNumbersAreCorrectForGame()
        {
            TheNumbersAreCorrectForGame(LottoGames[0].GameKey);
        }

        private void TheNumbersAreCorrectForGame(string gameKey)
        {
            var gameSetting = GameSettings[gameKey];
            ViewModel.LottoNumbers.Count(x => !x.IsBouns && x.Number >= gameSetting.Min && x.Number <= gameSetting.Max)
                .Should().Be(gameSetting.Count);
            ViewModel.LottoNumbers.Count(x => x.IsBouns && x.Number >= gameSetting.BonusNumberMin && x.Number <= gameSetting.BonusNumberMax).Should().Be(gameSetting.BonusNumberCount);
        }
    }
}
