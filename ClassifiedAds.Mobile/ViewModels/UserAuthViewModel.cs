using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.MemberRepoService;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using ClassifiedAds.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassifiedAds.Mobile.ViewModels
{
    public partial class UserAuthViewModel : ObservableObject
    {
        private readonly IUserAuthService _authService;
        private readonly IMemberService _memberService;

        // ============================
        // 1. LOGIN INPUTS
        // ============================
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;

        // ============================
        // 2. PROFILE DATA (Flattened)
        // ============================
        // These replace the nested 'CurrentUser' object. 
        // We separate 'ProfileEmail' from the input 'Email' to avoid confusion.
        [ObservableProperty] private string profileDisplayName;
        [ObservableProperty] private string profileEmail;
        [ObservableProperty] private string profileImageUrl;

        // ============================
        // 3. UI STATE
        // ============================
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string errorMessage;
        [ObservableProperty] private bool hasError;

        // ADD THIS PROPERTY to store the User's ID
        [ObservableProperty] private string currentUserId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
        private bool isLoggedIn;

        public bool IsNotLoggedIn => !IsLoggedIn;

        public UserAuthViewModel(IUserAuthService authService, IMemberService memberService)
        {
            _authService = authService;
            _memberService = memberService;
        }

        public async Task InitializeAsync()
        {
            // 1. Check if token exists in storage
            bool authenticated = await _authService.IsAuthenticatedAsync();

            if (authenticated)
            {
                // 2. Extract User ID from the token (offline friendly)
                var userId = await _authService.GetUserIdFromTokenAsync();

                if (!string.IsNullOrEmpty(userId))
                {
                    // 3. SET THE ID (This fixes the Chat Alignment)
                    CurrentUserId = userId;

                    // 4. Update the View State
                    IsLoggedIn = true;

                    // 5. Optional: Fetch latest Profile Image from API in background
                    // We wrap this in try/catch so it doesn't crash if offline
                    try
                    {
                        var profile = await _memberService.GetUserProfileAsync(userId);
                        if (profile != null)
                        {
                            ProfileDisplayName = profile.DisplayName;
                            ProfileImageUrl = !string.IsNullOrEmpty(profile.ImageUrl)
                                              ? profile.ImageUrl
                                              : "dotnet_bot.png";
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Profile fetch failed: {ex.Message}");
                    }
                }
                else
                {
                    // If token exists but we can't read the ID, force logout
                    await Logout();
                }
            }
            else
            {
                IsLoggedIn = false;
            }
        }


        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            HasError = false;
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;
                var user = await _authService.LoginAsync(Email, Password);

                if (user != null)
                {
                    // ========================================================
                    // MAPPING LOGIC (Similar to AdDetailViewModel)
                    // ========================================================
                    // We map DTO properties to ViewModel properties here.
                    // This lets us handle null checks and defaults in C# code.

                    // ADD THIS LINE: Store the ID
                    CurrentUserId = user.Id;

                    ProfileDisplayName = user.DisplayName;
                    ProfileEmail = user.Email;

                    // Image Logic: If null or empty, use default bot image
                    ProfileImageUrl = !string.IsNullOrEmpty(user.ImageUrl)
                                      ? user.ImageUrl
                                      : "dotnet_bot.png";

                    // Clear security fields
                    Password = string.Empty;

                    // Switch the View
                    IsLoggedIn = true;
                }
                else
                {
                    ErrorMessage = "Invalid Login Attempt. Check your credentials.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            await _authService.LogoutAsync();
            CurrentUserId = string.Empty; // Clear ID

            // Clear Profile Data
            ProfileDisplayName = string.Empty;
            ProfileEmail = string.Empty;
            ProfileImageUrl = string.Empty; // Or reset to default

            // Clear Inputs
            Email = string.Empty;
            Password = string.Empty;
            ErrorMessage = "";
            HasError = false;

            // Update State
            IsLoggedIn = false;
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.DisplayAlert("Register", "Navigate to Register Page", "OK");
        }

        // ... inside UserAuthViewModel class ...

        [RelayCommand]
        private async Task GoToEditProfile()
        {
            // Navigate to the registered route
            await Shell.Current.GoToAsync(nameof(EditProfilePage));
        }
    }
}