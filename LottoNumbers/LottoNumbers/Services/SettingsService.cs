using System.Collections.Generic;
using System.Linq;
using LottoNumbers.Models;
using LottoNumbers.Themes;
using Xamarin.Essentials.Interfaces;

namespace LottoNumbers.Services
{
    public interface ISettingsService
    {
        void SetString(string key, string value);
        string GetString(string key, string defaultValue);
        void SetBool(string key, bool value);
        bool GetBool(string key, bool defaultValue);
    }

    public class SettingsService : ISettingsService
    {
        private readonly IPreferences _preferences;

        public SettingsService(
            IPreferences preferences)
        {
            _preferences = preferences;
        }
        
        public bool GetBool(string key, bool defaultValue) => _preferences.Get(key, defaultValue);

        public string GetString(string key, string defaultValue) => _preferences.Get(key, defaultValue);

        public void SetBool(string key, bool value) => _preferences.Set(key, value);

        public void SetString(string key, string value) => _preferences.Set(key, value);
    }
}
