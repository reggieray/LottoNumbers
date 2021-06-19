using LottoNumbers.ViewModels;
using Moq;
using System.ComponentModel;

namespace LottoNumbers.UnitTests.ViewModels
{
    public abstract class ViewModelTestsBase<TViewModel> where TViewModel : ViewModelBase
    {
        protected TViewModel ViewModel;
        protected readonly Mock<PropertyChangedEventHandler> MockPropertyChangedEventHandler = new Mock<PropertyChangedEventHandler>();

        protected virtual void SetupOnPropertyChanged()
        {
            ViewModel.PropertyChanged += MockPropertyChangedEventHandler.Object;
        }
    }
}
