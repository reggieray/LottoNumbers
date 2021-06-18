using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LottoNumbers.Models;

namespace LottoNumbers.Services
{
    public interface ILottoGameService
    {
        Task FetchLatestConfigAsync();

        Task<IEnumerable<LottoNumber>> GenerateNumbersAsync(string gameKey);

        Task<IEnumerable<LottoGame>> GetGamesAsync();
    }

    public class LottoGameService : ILottoGameService
    {
        private const string GamesKey = "games";
        private const string GameSettingsKey = "game_settings";

        private readonly Random random = new Random();
        private readonly IRemoteConfigService _remoteConfigService;

        public LottoGameService(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        public async Task FetchLatestConfigAsync()
        {
            await _remoteConfigService.FetchAndActivateAsync();   
        }

        public async Task<IEnumerable<LottoNumber>> GenerateNumbersAsync(string gameKey)
        {
            var gameSettings = await _remoteConfigService.GetAsync<Dictionary<string, LottoGameSetting>>(GameSettingsKey);
            var gameSetting = gameSettings[gameKey];

            var lottoNumbers = new List<LottoNumber>();

            AddNumbers(lottoNumbers, gameSetting, MapNumber);

            if (gameSetting.HasBounsNumber)
            {
                AddBonusNumbers(lottoNumbers, gameSetting, MapBonusNumber);
            }

            return lottoNumbers.OrderBy(x => x.IsBouns).ThenBy(x => x.Number);
        }

        public async Task<IEnumerable<LottoGame>> GetGamesAsync()
        {
            return await _remoteConfigService.GetAsync<List<LottoGame>>(GamesKey);
        }

        private void AddNumbers(List<LottoNumber> lottoNumbers, LottoGameSetting gameSetting, Func<int, LottoGameSetting, LottoNumber> mapNumber)
        {
            while (lottoNumbers.Count(x => !x.IsBouns) < gameSetting.Count)
            {
                var number = random.Next(gameSetting.Min, gameSetting.Max);
                if (!lottoNumbers.Any(x => x.Number == number))
                {
                    lottoNumbers.Add(mapNumber(number, gameSetting));
                }
            }
        }

        private void AddBonusNumbers(List<LottoNumber> lottoNumbers, LottoGameSetting gameSetting, Func<int, LottoGameSetting, LottoNumber> mapNumber)
        {
            while (lottoNumbers.Count(x => x.IsBouns) < gameSetting.BonusNumberCount)
            {
                var number = random.Next(gameSetting.BonusNumberMin, gameSetting.BonusNumberMax);
                if (!lottoNumbers.Any(x => x.Number == number && x.IsBouns))
                {
                    lottoNumbers.Add(mapNumber(number, gameSetting));
                }
            }
        }

        private LottoNumber MapNumber(int number, LottoGameSetting gameSetting) => new LottoNumber { Number = number, BallColor = gameSetting.BallColor };

        private LottoNumber MapBonusNumber(int number, LottoGameSetting gameSetting) => new LottoNumber { Number = number, BallColor = gameSetting.BonusBallColor, IsBouns = true };
    }
}
