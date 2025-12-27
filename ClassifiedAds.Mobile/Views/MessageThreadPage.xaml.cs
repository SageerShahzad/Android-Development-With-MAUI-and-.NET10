using ClassifiedAds.Mobile.ViewModels;
using System.Collections.Specialized;

namespace ClassifiedAds.Mobile.Views;

public partial class MessageThreadPage : ContentPage
{
    private readonly MessageThreadViewModel _viewModel;

    public MessageThreadPage(MessageThreadViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Messages.CollectionChanged += OnMessagesChanged;

        // Initial Scroll
        if (_viewModel.Messages.Count > 0)
        {
            // Small delay to ensure layout is ready on Navigation
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                ScrollToBottom(false));
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
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // FIX: Dispatcher Delay (Report Section 4.4)
            // Wait for Android to measure the new item before scrolling
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                ScrollToBottom(true));
        }
    }

    private void ScrollToBottom(bool animate)
    {
        try
        {
            var count = _viewModel.Messages.Count;
            if (count > 0)
            {
                MessagesList.ScrollTo(count - 1, position: ScrollToPosition.End, animate: animate);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Scroll Error: {ex.Message}");
        }
    }
}