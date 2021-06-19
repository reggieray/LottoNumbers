using LottoNumbers.Services;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace LottoNumbers.UWP.Services
{
    public class UWPRemoteConfigService : IRemoteConfigService
    {
        public Task FetchAndActivateAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<TOutput> GetAsync<TOutput>(string key)
        {
            var resourceLoader = new ResourceLoader();
            var json = resourceLoader.GetString(key);

            return await Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<TOutput>(json));
        }
    }
}
