namespace LottoNumbers.Models
{
    public class LottoGameSetting
    {
        public int Count { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string BallColor { get; set; }
        public bool HasBounsNumber { get; set; }
        public int BonusNumberCount { get; set; }
        public int BonusNumberMin { get; set; }
        public int BonusNumberMax { get; set; }
        public string BonusBallColor { get; set; }
    }
}
