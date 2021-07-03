using FluentAssertions;
using LottoNumbers.Constants;
using LottoNumbers.ViewModels;
using Moq;
using System;
using TestStack.BDDfy;
using Xunit;

namespace LottoNumbers.UnitTests.ViewModels
{
    public class SettingsPageViewModelTests : ViewModelTestsBase<SettingsPageViewModel>
    {
        public SettingsPageViewModelTests()
        {
            ViewModel = new SettingsPageViewModel(mockNavigationService.Object, settingsService);
            this.SetupOnPropertyChanged();
        }

        [Fact]
        public void UserNavigatesToAndUpdatesSettings()
        {
            var savedUsePseudorandomSeed = default(bool);
            var savedPseudorandomDateSeed = default(DateTime);
            var usePseudorandomSeed = default(bool);
            var pseudorandomDateSeed = default(DateTime);
            var today = DateTime.Now;
            var yesterday = today.AddDays(-1);

            this.Given(_ => _.AUserNavigatedToTheSettingPageWithSavedSettings(savedUsePseudorandomSeed, savedPseudorandomDateSeed))
            .When(_ => _.TheUserUpdatesTheSettings(usePseudorandomSeed, pseudorandomDateSeed))
            .Then(_ => _.TheSettingsArePersisted(usePseudorandomSeed, pseudorandomDateSeed))
            .And(_ => _.TheSettingsAreDisplayed(usePseudorandomSeed, savedPseudorandomDateSeed, pseudorandomDateSeed))
            .WithExamples(new ExampleTable("savedUsePseudorandomSeed", "savedPseudorandomDateSeed", "usePseudorandomSeed", "pseudorandomDateSeed")
            {
                { false, today , true, yesterday },
                { true, yesterday, false, today }
            })
            .BDDfy();
        }

        private void TheSettingsAreDisplayed(bool usePseudorandomSeed, DateTime savedPseudorandomDateSeed, DateTime pseudorandomDateSeed)
        {
            ViewModel.Title.Should().Be("Settings");
            ViewModel.UsePseudorandomSeed.Should().Be(usePseudorandomSeed);
            var expectedPseudorandomDateSeed = usePseudorandomSeed ? pseudorandomDateSeed : DateTime.Parse(savedPseudorandomDateSeed.ToString(SettingConstants.DATE_FORMAT));
            ViewModel.PseudorandomDateSeed.Should().Be(expectedPseudorandomDateSeed);
        }

        private void AUserNavigatedToTheSettingPageWithSavedSettings(bool savedUsePseudorandomSeed, DateTime savedPseudorandomDateSeed)
        {
            mockPreferences.Reset();
            mockPreferences.Setup(x => x.Get(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, It.IsAny<bool>()))
               .Returns(savedUsePseudorandomSeed);
            mockPreferences.Setup(x => x.Get(SettingConstants.PSEUDORANDOM_SEED_KEY, It.IsAny<string>()))
               .Returns(savedPseudorandomDateSeed.ToString(SettingConstants.DATE_FORMAT));

            this.NavigatesToViewModel();
            ViewModel.OnNavigatedTo(mockNavigationParameters.Object);
        }

        private void TheUserUpdatesTheSettings(bool usePseudorandomSeed, DateTime pseudorandomDateSeed)
        {
            ViewModel.UsePseudorandomSeed = usePseudorandomSeed;
            if (usePseudorandomSeed)
            {
                ViewModel.PseudorandomDateSeed = pseudorandomDateSeed;
            }
        }

        private void TheSettingsArePersisted(bool usePseudorandomSeed, DateTime pseudorandomDateSeed)
        {
            mockPreferences.Verify(c => c.Set(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, usePseudorandomSeed), Times.Once);
            var dateSeedInvocationCount = usePseudorandomSeed ? 1 : 0;
            mockPreferences.Verify(c => c.Set(SettingConstants.PSEUDORANDOM_SEED_KEY, pseudorandomDateSeed.ToString(SettingConstants.DATE_FORMAT)), Times.Exactly(dateSeedInvocationCount));
        }
    }
}
