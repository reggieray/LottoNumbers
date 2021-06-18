using LottoNumbers.Services;
using LottoNumbers.ViewModels;
using Moq;
using Prism.Navigation;
using TestStack.BDDfy;
using Xunit;

namespace LottoNumbers.UnitTests.ViewModels
{
    public class MainPageViewModelTests : ViewModelTestsBase<MainPageViewModel>
    {
        private readonly ILottoGameService lottoGameService;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<IRemoteConfigService> mockRemoteConfigService;
        private readonly Mock<INavigationParameters> mockNavigationParameters;

        public MainPageViewModelTests()
        {
            mockNavigationParameters = new Mock<INavigationParameters>();
            mockNavigationService = new Mock<INavigationService>();
            lottoGameService = new LottoGameService(mockRemoteConfigService.Object);
            viewModel = new MainPageViewModel(mockNavigationService.Object, lottoGameService);
            this.SetupOnPropertyChanged();
        }

        [Fact]
        public void UserGeneratesLottoNumbers()
        {
            /*
            this.Given(_ => _.AUserNavigatesToTheMainPage())
            .When(s => s.WhenTheAccountHolderRequests(20))
            .Then(s => s.CardIsRetained(true), "Then the ATM should retain the card")
            .And(s => s.AndTheAtmShouldSayTheCardHasBeenRetained())
            .BDDfy();
            */
        }

        private void AUserNavigatesToTheMainPage()
        {
            viewModel.Initialize(mockNavigationParameters.Object);
        }
    }
}
