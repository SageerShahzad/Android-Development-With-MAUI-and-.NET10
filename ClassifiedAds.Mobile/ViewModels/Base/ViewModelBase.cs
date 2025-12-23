//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;

//namespace ClassifiedAds.Mobile.ViewModels.Base;

//public partial class ViewModelBase : ObservableValidator, IViewModelBase
//{
//    [ObservableProperty] 
//    private bool _isLoading;

//    public IAsyncRelayCommand InitializeAsyncCommand { get; }

//    public ViewModelBase()
//    {
//        InitializeAsyncCommand = new AsyncRelayCommand(
//            async () =>
//            {
//                IsLoading = true;
//                await Loading(LoadAsync);
//                IsLoading = false;
//            });
//    }

//    protected async Task Loading(Func<Task> unitOfWork)
//    {
//        await unitOfWork();
//    }

//    public virtual Task LoadAsync()
//    {
//        return Task.CompletedTask;
//    }
//}
