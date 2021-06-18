using System.Threading.Tasks;
using Firebase.RemoteConfig;
using LottoNumbers.Services;

namespace LottoNumbers.Droid.Services
{
    public class AndroidRemoteConfigService : IRemoteConfigService
    {
        public AndroidRemoteConfigService()
        {
            FirebaseRemoteConfig.Instance.SetDefaultsAsync(Resource.Xml.RemoteConfigDefaults);
        }

        public async Task FetchAndActivateAsync()
        {
            //Fetch remote values
            await FirebaseRemoteConfig.Instance.FetchAsync();

            //Activate new values
            FirebaseRemoteConfig.Instance.Activate();
        }

        public async Task<TOutput> GetAsync<TOutput>(string key)
        {
            var settings = FirebaseRemoteConfig.Instance.GetString(key);

            return await Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<TOutput>(settings));
        }
    }
}
