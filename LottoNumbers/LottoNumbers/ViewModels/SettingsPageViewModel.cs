using System;
using System.Collections.Generic;
using LottoNumbers.Constants;
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
                if (_selectedTheme != null)
                {
                    _settingsService.SetTheme(_selectedTheme.Type);
                }
            }
        }

        private bool _usePseudorandomSeed;
        public bool UsePseudorandomSeed
        {
            get { return _usePseudorandomSeed; }
            set
            {
                SetProperty(ref _usePseudorandomSeed, value);
                _settingsService.SetBool(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, value);
            }
        }

        private DateTime _pseudorandomDateSeed;
        public DateTime PseudorandomDateSeed
        {
            get { return _pseudorandomDateSeed; }
            set
            {
                if (value.ToString(SettingConstants.DATE_FORMAT) == SettingConstants.DEFAULT_MIN_DATE) return;
                SetProperty(ref _pseudorandomDateSeed, value);
                _settingsService.SetString(SettingConstants.PSEUDORANDOM_SEED_KEY, value.ToString(SettingConstants.DATE_FORMAT));
            }
        }

        public SettingsPageViewModel(
           INavigationService navigationService,
           ISettingsService settingsService)
           : base(navigationService)
        {
            Title = "Settings";
            _settingsService = settingsService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            UsePseudorandomSeed = _settingsService.GetBool(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, false);
            var pseudorandomDateSeed = _settingsService.GetString(SettingConstants.PSEUDORANDOM_SEED_KEY, DateTime.Now.ToString(SettingConstants.DATE_FORMAT));
            PseudorandomDateSeed = DateTime.Parse(pseudorandomDateSeed);
        }
    }
}
