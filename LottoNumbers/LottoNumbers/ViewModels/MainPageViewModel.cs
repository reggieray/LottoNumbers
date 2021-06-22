using System.Collections.Generic;
using LottoNumbers.Models;
using LottoNumbers.Services;
using LottoNumbers.Views;
using Prism.Commands;
using Prism.Navigation;

namespace LottoNumbers.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ILottoGameService _lottoNumberService;

        private bool _showLuckyCat = true;
        public bool ShowLuckyCat
        {
            get { return _showLuckyCat; }
            set { SetProperty(ref _showLuckyCat, value); }
        }

        private LottoGame _selectedGame;
        public LottoGame SelectedGame
        {
            get { return _selectedGame; }
            set { SetProperty(ref _selectedGame, value); }
        }

        private List<LottoGame> _lottoGames;
        public List<LottoGame> LottoGames
        {
            get { return _lottoGames; }
            set { SetProperty(ref _lottoGames, value); }
        }

        private List<LottoNumber> _lottoNumbers;
        public List<LottoNumber> LottoNumbers
        {
            get { return _lottoNumbers; }
            set { SetProperty(ref _lottoNumbers, value); }
        }

        public DelegateCommand GenerateNumbersCommand { get; }

        public DelegateCommand SettingsNavigationCommand { get; }

        public DelegateCommand ShowLuckyCatCommand { get; }

        public MainPageViewModel(
            INavigationService navigationService,
            ILottoGameService lottoNumberService)
            : base(navigationService)
        {
            _lottoNumberService = lottoNumberService;
            GenerateNumbersCommand = new DelegateCommand(OnGenerateNumbers, CanGenerateNumbers).ObservesProperty(() => SelectedGame);
            SettingsNavigationCommand = new DelegateCommand(OnNavigateToSettings);
            ShowLuckyCatCommand = new DelegateCommand(OnShowLuckyCat);
        }

        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            await _lottoNumberService.FetchLatestConfigAsync();
            var lottoGames = await _lottoNumberService.GetGamesAsync();
            LottoGames = new List<LottoGame>(lottoGames);
        }

        private bool CanGenerateNumbers()
        {
            return _selectedGame != null;
        }

        private async void OnGenerateNumbers()
        {
            ShowLuckyCat = false;
            var lottoNumbers = await _lottoNumberService.GenerateNumbersAsync(_selectedGame.GameKey);
            LottoNumbers = new List<LottoNumber>(lottoNumbers);
        }

        private async void OnNavigateToSettings()
        {
            await NavigationService.NavigateAsync($"{nameof(SettingsPage)}");
        }

        private void OnShowLuckyCat()
        {
            ShowLuckyCat = true;
        }
    }
}