using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.MemberRepoService;
using ClassifiedAds.Mobile.RepoServices.MessageRepoService;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using ClassifiedAds.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClassifiedAds.Mobile.ViewModels
{
    [QueryProperty(nameof(RecipientId), "RecipientId")]
    public partial class MessageThreadViewModel : ObservableObject
    {
        private readonly IMessageService _messageService;
        private readonly IMemberService _memberService;
        private readonly UserAuthViewModel _userAuthViewModel;
        private readonly SignalRService _signalRService;

        [ObservableProperty] private string recipientId;
        [ObservableProperty] private string newMessageContent;
        [ObservableProperty] private bool isBusy;

        private string _recipientImageUrl = "dotnet_bot.png";
        private string _myImageUrl = "dotnet_bot.png";

        public ObservableCollection<MessageUiModel> Messages { get; } = new();

        public MessageThreadViewModel(
            IMessageService messageService,
            IMemberService memberService,
            UserAuthViewModel userAuthViewModel,
            SignalRService signalRService)
        {
            _messageService = messageService;
            _memberService = memberService;
            _userAuthViewModel = userAuthViewModel;
            _signalRService = signalRService;

            _signalRService.OnMessageReceived += HandleNewMessage;
        }

        public async Task OnDisappearing()
        {
            await _signalRService.DisconnectAsync();
        }

        partial void OnRecipientIdChanged(string value)
        {
            InitializeChat();
        }

        private async void InitializeChat()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await _signalRService.ConnectAsync(RecipientId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR Error: {ex.Message}");
            }

            await LoadHistory();
            IsBusy = false;
        }

        private async Task LoadHistory()
        {
            // 1. Fetch Profile and Thread concurrently
            var profileTask = _memberService.GetUserProfileAsync(RecipientId);
            var threadTask = _messageService.GetMessageThreadAsync(RecipientId);

            await Task.WhenAll(profileTask, threadTask);

            var recipientProfile = profileTask.Result;
            var thread = threadTask.Result;

            // 2. Setup Images
            _recipientImageUrl = !string.IsNullOrEmpty(recipientProfile?.ImageUrl) ? recipientProfile.ImageUrl : "dotnet_bot.png";
            _myImageUrl = !string.IsNullOrEmpty(_userAuthViewModel.ProfileImageUrl) ? _userAuthViewModel.ProfileImageUrl : "dotnet_bot.png";

            var currentUserId = _userAuthViewModel.CurrentUserId;

            // 3. Clear and Populate
            // Optimally, we would use ObservableRangeCollection here to prevent flickering,
            // but clearing and re-adding is standard if you don't want extra NuGets.
            Messages.Clear();
            foreach (var msg in thread)
            {
                AddMessageToUi(msg, currentUserId);
            }
        }

        private void HandleNewMessage(MessageDto msg)
        {
            var currentUserId = _userAuthViewModel.CurrentUserId;
            AddMessageToUi(msg, currentUserId);
        }

        private void AddMessageToUi(MessageDto msg, string currentUserId)
        {
            // Case-Insensitive ID Check
            bool isMe = string.Equals(msg.SenderId, currentUserId, StringComparison.OrdinalIgnoreCase);

            Messages.Add(new MessageUiModel
            {
                Content = msg.Content,
                MessageSent = msg.MessageSent,
                DateRead = msg.DateRead,
                IsMine = isMe,
                SenderDisplayName = isMe ? "Me" : msg.SenderDisplayName,
                SenderImageUrl = isMe ? _myImageUrl : _recipientImageUrl
            });
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageContent)) return;

            var contentToSend = NewMessageContent;
            NewMessageContent = string.Empty;

            var createDto = new CreateMessageDto
            {
                RecipientId = RecipientId,
                Content = contentToSend
            };

            try
            {
                await _signalRService.SendMessageAsync(createDto);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Failed to send message", "OK");
            }
        }
    }

    public class MessageUiModel
    {
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; }
        public bool IsMine { get; set; }
        public string SenderDisplayName { get; set; }
        public string SenderImageUrl { get; set; }

        public LayoutOptions Alignment => IsMine ? LayoutOptions.End : LayoutOptions.Start;
        public Color BubbleColor => IsMine ? Color.FromArgb("#5243E4") : Color.FromArgb("#F2F2F2");
        public Color TextColor => IsMine ? Colors.White : Colors.Black;

        public string StatusText
        {
            get
            {
                if (IsMine)
                {
                    return DateRead.HasValue ? "Seen" : "Delivered";
                }
                return "";
            }
        }
    }
}