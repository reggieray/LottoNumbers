using Xamarin.Forms;

namespace LottoNumbers.Services
{
    public interface IApplicationService
    {
        void ClearMergedDictionaries();
        void AddResourceDictionary<T>(T resourceDictionary) where T : ResourceDictionary;
    }

    public class ApplicationService : IApplicationService
    {
        public void AddResourceDictionary<T>(T resourceDictionary) where T : ResourceDictionary
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
                mergedDictionaries.Add(resourceDictionary);
        }

        public void ClearMergedDictionaries()
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
                mergedDictionaries.Clear();
        }
    }
}
