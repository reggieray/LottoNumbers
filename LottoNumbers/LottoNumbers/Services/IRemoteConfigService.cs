using System.Threading.Tasks;

namespace LottoNumbers.Services
{
    public interface IRemoteConfigService
    {
        Task FetchAndActivateAsync();

        Task<IOutput> GetAsync<IOutput>(string key);
    }
}
