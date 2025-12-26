using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClassifiedAds.Mobile.Services
{
    public class SignalRService
    {
        private readonly IUserAuthService _authService;
        private HubConnection? _hubConnection;

        public event Action<MessageDto>? OnMessageReceived;

        public SignalRService(IUserAuthService authService)
        {
            _authService = authService;
        }

        public async Task ConnectAsync(string recipientId)
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                return;

            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return;

            // Determine Base URL (Android vs Windows)
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5000/hubs/messages"
                : "https://localhost:5001/hubs/messages";

            // Your Backend requires '?userId=' in the Query String to identify the chat thread
            var hubUrl = $"{baseUrl}?userId={recipientId}";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            // LISTENERS
            // 1. Listen for new messages
            _hubConnection.On<MessageDto>("NewMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnMessageReceived?.Invoke(message);
                });
            });

            // 2. Listen for the full thread load (Backend sends this OnConnected)
            // We can also listen to "ReceiveMessageThread" if we want to load via SignalR
            // but usually, we just listen for new messages.

            await _hubConnection.StartAsync();
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public async Task SendMessageAsync(CreateMessageDto messageDto)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;

            // Call the 'SendMessage' method defined in your Backend MessageHub.cs
            await _hubConnection.InvokeAsync("SendMessage", messageDto);
        }
    }
}