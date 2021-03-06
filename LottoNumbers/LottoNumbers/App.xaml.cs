using LottoNumbers.Services;
using LottoNumbers.ViewModels;
using LottoNumbers.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace LottoNumbers
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync($"NavigationPage/{nameof(MainPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            //Pages
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();

            //Services
            containerRegistry.RegisterSingleton<ILottoGameService, LottoGameService>();
            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();

            //Xamarin Essentials
            containerRegistry.RegisterSingleton<IPreferences, PreferencesImplementation>();
        }
    }
}
