using CommunityToolkit.Mvvm.Input;

namespace ClassifiedAds.Mobile.ViewModels.Base;

public interface IViewModelBase
{
    IAsyncRelayCommand InitializeAsyncCommand { get; }
}
