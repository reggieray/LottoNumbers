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

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
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

        public MainPageViewModel(
            INavigationService navigationService,
            ILottoGameService lottoNumberService)
            : base(navigationService)
        {
            _lottoNumberService = lottoNumberService;
            GenerateNumbersCommand = new DelegateCommand(OnGenerateNumbers, CanGenerateNumbers).ObservesProperty(() => SelectedGame);
            SettingsNavigationCommand = new DelegateCommand(OnNavigateToSettings);
        }

        public async override void Initialize(INavigationParameters parameters)
        {
            IsLoading = true;
            base.Initialize(parameters);
            await _lottoNumberService.FetchLatestConfigAsync();
            var lottoGames = await _lottoNumberService.GetGamesAsync();
            LottoGames = new List<LottoGame>(lottoGames);
            IsLoading = false;
        }

        private bool CanGenerateNumbers()
        {
            return _selectedGame != null;
        }

        private async void OnGenerateNumbers()
        {
            IsLoading = true;
            var lottoNumbers = await _lottoNumberService.GenerateNumbersAsync(_selectedGame.GameKey);
            LottoNumbers = new List<LottoNumber>(lottoNumbers);
            IsLoading = false;
        }

        private async void OnNavigateToSettings()
        {
            await NavigationService.NavigateAsync($"{nameof(SettingsPage)}");
        }
    }
}