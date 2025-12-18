using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassifiedAds.Mobile.ViewModels;

// INHERIT FROM ObservableObject (This fixes the error)
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string title;
}