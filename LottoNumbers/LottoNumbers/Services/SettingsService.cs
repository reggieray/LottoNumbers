using System.Collections.Generic;
using System.Linq;
using LottoNumbers.Models;
using LottoNumbers.Themes;
using Xamarin.Essentials.Interfaces;

namespace LottoNumbers.Services
{
    public interface ISettingsService
    {
        List<Theme> GetThemes();
        Theme GetCurrentTheme();
        void SetTheme(ThemeType type);
        void LoadTheme();
    }

    public class SettingsService : ISettingsService
    {
        private const string CURRENT_THEME_KEY = "theme";

        private readonly IPreferences _preferences;
        private readonly IApplicationService _applicationService;

        public SettingsService(
            IPreferences preferences,
            IApplicationService applicationService)
        {
            _preferences = preferences;
            _applicationService = applicationService;
        }

        public Theme GetCurrentTheme()
        {
            var savedTheme = (ThemeType)_preferences.Get(CURRENT_THEME_KEY, (int)ThemeType.Light);
            return GetThemes().Single(x => x.Type == savedTheme);
        }

        public List<Theme> GetThemes()
        {
            return new List<Theme>
            {
                new Theme { Name = ThemeType.Dark.ToString(), Type = ThemeType.Dark },
                new Theme { Name = ThemeType.Light.ToString(), Type = ThemeType.Light }
            };
        }

        public void LoadTheme() => SetTheme(GetCurrentTheme().Type);

        public void SetTheme(ThemeType type)
        {
            _applicationService.ClearMergedDictionaries();
            switch (type)
            {
                case ThemeType.Light:
                    _applicationService.AddResourceDictionary(new LightTheme());
                    break;
                case ThemeType.Dark:
                    _applicationService.AddResourceDictionary(new DarkTheme());
                    break;
            }
            _preferences.Set(CURRENT_THEME_KEY, (int)type);
        }
    }
}
