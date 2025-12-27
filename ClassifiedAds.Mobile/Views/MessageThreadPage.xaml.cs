using ClassifiedAds.Mobile.ViewModels;
using System.Collections.Specialized;

namespace ClassifiedAds.Mobile.Views;

public partial class MessageThreadPage : ContentPage
{
    private MessageThreadViewModel _viewModel;

    public MessageThreadPage(MessageThreadViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Subscribe to collection changes
        _viewModel.Messages.CollectionChanged += OnMessagesChanged;

        // Scroll to bottom initially if items exist
        if (_viewModel.Messages.Count > 0)
        {
            ScrollToBottom(false);
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Messages.CollectionChanged -= OnMessagesChanged;

        if (BindingContext is MessageThreadViewModel vm)
        {
            await vm.OnDisappearing();
        }
    }

    private void OnMessagesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Only scroll if items were added
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ScrollToBottom(true);
        }
    }

    private void ScrollToBottom(bool animate)
    {
        // Must run on Main Thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var count = _viewModel.Messages.Count;
            if (count > 0)
            {
                // Find the CollectionView in your XAML. 
                // GIVE YOUR COLLECTIONVIEW x:Name="MessagesList"
                MessagesList.ScrollTo(count - 1, position: ScrollToPosition.End, animate: animate);
            }
        });
    }
}