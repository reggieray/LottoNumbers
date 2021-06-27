using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LottoNumbers.Constants;
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

        private readonly IRemoteConfigService _remoteConfigService;
        private readonly ISettingsService _settingsService;

        public LottoGameService(
            IRemoteConfigService remoteConfigService,
            ISettingsService settingsService)
        {
            _remoteConfigService = remoteConfigService;
            _settingsService = settingsService;
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
            var random = GetRandom();
            while (lottoNumbers.Count(x => !x.IsBouns) < gameSetting.Count)
            {
                var number = random.Next(gameSetting.Min, gameSetting.Max + 1);
                if (!lottoNumbers.Any(x => x.Number == number))
                {
                    lottoNumbers.Add(mapNumber(number, gameSetting));
                }
            }
        }

        private void AddBonusNumbers(List<LottoNumber> lottoNumbers, LottoGameSetting gameSetting, Func<int, LottoGameSetting, LottoNumber> mapNumber)
        {
            var random = GetRandom();
            while (lottoNumbers.Count(x => x.IsBouns) < gameSetting.BonusNumberCount)
            {
                var number = random.Next(gameSetting.BonusNumberMin, gameSetting.BonusNumberMax + 1);
                if (!lottoNumbers.Any(x => x.Number == number && x.IsBouns))
                {
                    lottoNumbers.Add(mapNumber(number, gameSetting));
                }
            }
        }

        private Random GetRandom()
        {
            var usePseudorandomSeed = _settingsService.GetBool(SettingConstants.USE_PSEUDORANDOM_SEED_KEY, false);
            var random = usePseudorandomSeed ? new Random(GetSeed()) : new Random();
            return random;
        }

        private int GetSeed()
        {
            var date = DateTime.Parse(_settingsService.GetString(SettingConstants.PSEUDORANDOM_SEED_KEY, DateTime.Now.ToString(SettingConstants.DATE_FORMAT)));
            return (int)date.Ticks;
        }

        private LottoNumber MapNumber(int number, LottoGameSetting gameSetting) => new LottoNumber { Number = number, BallColor = gameSetting.BallColor };

        private LottoNumber MapBonusNumber(int number, LottoGameSetting gameSetting) => new LottoNumber { Number = number, BallColor = gameSetting.BonusBallColor, IsBouns = true };
    }
}
