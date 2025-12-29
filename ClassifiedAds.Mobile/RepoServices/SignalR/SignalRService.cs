using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClassifiedAds.Mobile.Services
{
    public class SignalRService
    {
        private readonly IUserAuthService _authService;
        private HubConnection? _hubConnection;      // Chat
        private HubConnection? _presenceConnection; // Online Status

        // EVENTS
        public event Action<MessageDto>? OnMessageReceived;
        public event Action<string>? OnUserOnline;
        public event Action<string>? OnUserOffline;
        public event Action<string[]>? OnOnlineUsersReceived;

        public SignalRService(IUserAuthService authService)
        {
            _authService = authService;
        }

        public async Task ConnectAsync(string recipientId)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return;

            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://localhost:5000" // Requires 'adb reverse'
                : "https://localhost:5001";

            // 1. CONNECT TO PRESENCE HUB (Tracks Online/Offline)
            if (_presenceConnection == null || _presenceConnection.State != HubConnectionState.Connected)
            {
                _presenceConnection = new HubConnectionBuilder()
                    .WithUrl($"{baseUrl}/hubs/presence", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(token);
                    })
                    .WithAutomaticReconnect()
                    .Build();

                // Match these names exactly with your Backend 'PresenceHub.cs'
                _presenceConnection.On<string>("UserOnline", userId =>
                    MainThread.BeginInvokeOnMainThread(() => OnUserOnline?.Invoke(userId)));

                _presenceConnection.On<string>("UserOffline", userId =>
                    MainThread.BeginInvokeOnMainThread(() => OnUserOffline?.Invoke(userId)));

                _presenceConnection.On<string[]>("GetOnlineUsers", userIds =>
                    MainThread.BeginInvokeOnMainThread(() => OnOnlineUsersReceived?.Invoke(userIds)));

                await _presenceConnection.StartAsync();
            }

            // 2. CONNECT TO MESSAGE HUB (Chat)
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
            {
                var hubUrl = $"{baseUrl}/hubs/messages?userId={recipientId}";

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(token);
                    })
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<MessageDto>("NewMessage", (message) =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        OnMessageReceived?.Invoke(message);
                    });
                });

                await _hubConnection.StartAsync();
            }
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
            if (_presenceConnection != null)
            {
                await _presenceConnection.StopAsync();
                await _presenceConnection.DisposeAsync();
                _presenceConnection = null;
            }
        }

        public async Task SendMessageAsync(CreateMessageDto messageDto)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;
            await _hubConnection.InvokeAsync("SendMessage", messageDto);
        }
    }
}