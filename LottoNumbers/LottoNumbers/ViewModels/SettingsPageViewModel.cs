using System.Collections.Generic;
using LottoNumbers.Models;
using LottoNumbers.Services;
using Prism.Navigation;

namespace LottoNumbers.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        public List<Theme> Themes { get; set; }

        private Theme _selectedTheme;
        public Theme SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                SetProperty(ref _selectedTheme, value);
                _settingsService.SetTheme(_selectedTheme.Type);
            }
        }

        public SettingsPageViewModel(
           INavigationService navigationService,
           ISettingsService settingsService)
           : base(navigationService)
        {
            _settingsService = settingsService;
            Themes = _settingsService.GetThemes();
        }
    }
}
