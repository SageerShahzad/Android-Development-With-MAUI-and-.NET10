using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.MemberRepoService;
using ClassifiedAds.Mobile.RepoServices.MessageRepoService;
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
        private readonly SignalRService _signalRService; // NEW

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
            SignalRService signalRService) // Inject SignalR
        {
            _messageService = messageService;
            _memberService = memberService;
            _userAuthViewModel = userAuthViewModel;
            _signalRService = signalRService;

            // Subscribe to SignalR events
            _signalRService.OnMessageReceived += HandleNewMessage;
        }

        // Cleanup when leaving
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

            // 1. Connect to SignalR
            try
            {
                await _signalRService.ConnectAsync(RecipientId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR Connection Failed: {ex.Message}");
            }

            // 2. Load Initial History via API (Standard practice: Load history via API, updates via SignalR)
            await LoadHistory();

            IsBusy = false;
        }

        private async Task LoadHistory()
        {
            // ... (Keep your existing LoadData logic to get photos and fetch initial list) ...
            // Be sure to set _myImageUrl and _recipientImageUrl here

            // (Simplified for brevity - ensure you fetch the thread from _messageService here)
            var profile = await _memberService.GetUserProfileAsync(RecipientId);
            if (profile != null) _recipientImageUrl = profile.ImageUrl ?? "dotnet_bot.png";
            _myImageUrl = _userAuthViewModel.ProfileImageUrl ?? "dotnet_bot.png";

            var thread = await _messageService.GetMessageThreadAsync(RecipientId);
            var currentUserId = _userAuthViewModel.CurrentUserId;

            Messages.Clear();
            foreach (var msg in thread)
            {
                AddMessageToUi(msg, currentUserId);
            }
        }

        private void HandleNewMessage(MessageDto msg)
        {
            // This runs whenever SignalR receives a "NewMessage" event
            var currentUserId = _userAuthViewModel.CurrentUserId;
            AddMessageToUi(msg, currentUserId);

            // Auto-scroll logic needs to be triggered here in the View
        }

        // Inside MessageThreadViewModel.cs

        private async void LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 1. Fetch Profile & Thread in parallel
                var profileTask = _memberService.GetUserProfileAsync(RecipientId);
                var threadTask = _messageService.GetMessageThreadAsync(RecipientId);

                await Task.WhenAll(profileTask, threadTask);

                var recipientProfile = profileTask.Result;
                var thread = threadTask.Result;

                // 2. Set Images with Fallbacks
                // Use the profile image if valid, otherwise fallback to bot
                _recipientImageUrl = (!string.IsNullOrEmpty(recipientProfile?.ImageUrl))
                                     ? recipientProfile.ImageUrl
                                     : "dotnet_bot.png";

                // Get "My" image from the global auth state
                _myImageUrl = (!string.IsNullOrEmpty(_userAuthViewModel.ProfileImageUrl))
                              ? _userAuthViewModel.ProfileImageUrl
                              : "dotnet_bot.png";

                var currentUserId = _userAuthViewModel.CurrentUserId;

                Messages.Clear();
                foreach (var msg in thread)
                {
                    AddMessageToUi(msg, currentUserId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chat Load Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddMessageToUi(MessageDto msg, string currentUserId)
        {
            bool isMe = string.Equals(msg.SenderId, currentUserId, StringComparison.OrdinalIgnoreCase);

            Messages.Add(new MessageUiModel
            {
                Content = msg.Content,
                MessageSent = msg.MessageSent,
                DateRead = msg.DateRead, // Map the read date
                IsMine = isMe,

                // Map the Name
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

          

            // Use SignalR to send. The Backend Hub will broadcast "NewMessage" back to us.
            // So we don't strictly need to add it to the list manually here, 
            // BUT adding it manually makes the UI feel faster (Optimistic UI).
            try
            {
                await _signalRService.SendMessageAsync(createDto);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Failed to send via SignalR", "OK");
            }

            Messages.Add(new MessageUiModel {  });
        }
    }

    public class MessageUiModel
    {
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; } // NEW: To track seen status
        public bool IsMine { get; set; }
        public string SenderDisplayName { get; set; } // NEW: Name of sender
        public string SenderImageUrl { get; set; }

        // --- UI HELPERS ---

        public LayoutOptions Alignment => IsMine ? LayoutOptions.End : LayoutOptions.Start;
        public Color BubbleColor => IsMine ? Color.FromArgb("#5243E4") : Color.FromArgb("#F2F2F2"); // Purple vs Grey
        public Color TextColor => IsMine ? Colors.White : Colors.Black;
        public int AvatarColumn => IsMine ? 2 : 0;

        // NEW: Generate the status text (Angular Parity)
        public string StatusText
        {
            get
            {
                if (IsMine)
                {
                    // If I sent it, show if it was read or just delivered
                    return DateRead.HasValue ? "Seen" : "Delivered";
                    // Note: For "Seen 1 hour ago", you would need a 'TimeAgo' helper, 
                    // but "Seen" is sufficient for MVP.
                }
                return ""; // Don't show status for received messages (usually)
            }
        }
    }
}


