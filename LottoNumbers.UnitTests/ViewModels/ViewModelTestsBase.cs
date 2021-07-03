using LottoNumbers.Services;
using LottoNumbers.ViewModels;
using Moq;
using Prism.Navigation;
using System.ComponentModel;
using Xamarin.Essentials.Interfaces;

namespace LottoNumbers.UnitTests.ViewModels
{
    public abstract class ViewModelTestsBase<TViewModel> where TViewModel : ViewModelBase
    {
        protected ISettingsService settingsService;
        protected Mock<IPreferences> mockPreferences;
        protected Mock<INavigationService> mockNavigationService;
        protected Mock<INavigationParameters> mockNavigationParameters;
        protected TViewModel ViewModel;
        protected readonly Mock<PropertyChangedEventHandler> MockPropertyChangedEventHandler = new Mock<PropertyChangedEventHandler>();

        public ViewModelTestsBase()
        {
            mockPreferences = new Mock<IPreferences>();
            mockNavigationService = new Mock<INavigationService>();
            mockNavigationParameters = new Mock<INavigationParameters>();
            settingsService = new SettingsService(mockPreferences.Object);
        }

        protected virtual void SetupOnPropertyChanged()
        {
            ViewModel.PropertyChanged += MockPropertyChangedEventHandler.Object;
        }

        protected void NavigatesToViewModel()
        {
            MockPropertyChangedEventHandler.Invocations.Clear();
            ViewModel.Initialize(mockNavigationParameters.Object);
        }
    }
}
