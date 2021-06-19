using System.Threading.Tasks;

namespace LottoNumbers.Services
{
    public interface IRemoteConfigService
    {
        Task FetchAndActivateAsync();

        Task<TOutput> GetAsync<TOutput>(string key);
    }
}
