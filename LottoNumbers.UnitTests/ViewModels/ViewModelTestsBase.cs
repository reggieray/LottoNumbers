using LottoNumbers.ViewModels;
using Moq;
using System.ComponentModel;

namespace LottoNumbers.UnitTests.ViewModels
{
    public abstract class ViewModelTestsBase<TViewModel> where TViewModel : ViewModelBase
    {
        protected TViewModel viewModel;

        Mock<PropertyChangedEventHandler> mockPropertyChangedEventHandler ;

        protected virtual void SetupOnPropertyChanged()
        {
            viewModel.PropertyChanged += mockPropertyChangedEventHandler.Object;
        }
    }
}
