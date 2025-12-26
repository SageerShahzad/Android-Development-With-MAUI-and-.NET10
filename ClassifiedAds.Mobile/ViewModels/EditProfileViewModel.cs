using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.MemberRepoService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassifiedAds.Mobile.ViewModels
{
    public partial class EditProfileViewModel : ObservableObject
    {
        private readonly IMemberService _memberService;
        private readonly UserAuthViewModel _userAuthViewModel;

        [ObservableProperty] private string displayName;
        [ObservableProperty] private string description;
        [ObservableProperty] private string city;
        [ObservableProperty] private string country;

        // Image Display
        [ObservableProperty] private ImageSource profileImageSource;

        // Logic
        [ObservableProperty] private bool isBusy;
        private FileResult? _selectedImageFile;

        //public EditProfileViewModel(IMemberService memberService, UserAuthViewModel userAuthViewModel)
        public EditProfileViewModel(IMemberService memberService, UserAuthViewModel userAuthViewModel)
        {
            _memberService = memberService;
            _userAuthViewModel = userAuthViewModel;

            // Safety Wrapper
            try
            {
                LoadFullProfileData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing VM: {ex.Message}");
            }
        }

        private async void LoadFullProfileData()
        {
            if (IsBusy) return;
            IsBusy = true;

            // 1. Pre-fill basic data we already have (for instant UI feedback)
            DisplayName = _userAuthViewModel.ProfileDisplayName;
            var url = _userAuthViewModel.ProfileImageUrl;
            ProfileImageSource = !string.IsNullOrEmpty(url) ? url : "dotnet_bot.png";

            // 2. Fetch detailed data (City, Country, Bio) from API
            var userId = _userAuthViewModel.CurrentUserId;

            if (!string.IsNullOrEmpty(userId))
            {
                var fullProfile = await _memberService.GetUserProfileAsync(userId);

                if (fullProfile != null)
                {
                    // 3. Populate fields with fresh data from server
                    DisplayName = fullProfile.DisplayName; // Refresh in case it changed
                    Description = fullProfile.Description;
                    City = fullProfile.City;
                    Country = fullProfile.Country;

                    // Update image if server has a different one
                    if (!string.IsNullOrEmpty(fullProfile.ImageUrl))
                    {
                        ProfileImageSource = fullProfile.ImageUrl;
                    }
                }
            }

            IsBusy = false;
        }

        private void LoadCurrentData()
        {
            // FIX: Read from the flattened properties instead of the missing 'CurrentUser' object
            if (_userAuthViewModel.IsLoggedIn)
            {
                DisplayName = _userAuthViewModel.ProfileDisplayName;

                // If the URL is valid, use it; otherwise use default
                var url = _userAuthViewModel.ProfileImageUrl;
                ProfileImageSource = !string.IsNullOrEmpty(url) ? url : "dotnet_bot.png";

                // Note: City and Country are not currently stored in the simple Login Login response.
                // To populate them here, you would typically make a call to 'api/members/current' 
                // inside 'InitializeAsync' or here. For now, they start empty.
            }
        }

        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    _selectedImageFile = result;
                    ProfileImageSource = ImageSource.FromFile(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Could not pick image: " + ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            if (IsBusy) return;
            IsBusy = true;

            var updateDto = new MemberUpdateDto
            {
                DisplayName = DisplayName,
                Description = Description,
                City = City,
                Country = Country
            };

            var result = await _memberService.UpdateProfileAsync(updateDto, _selectedImageFile);

            IsBusy = false;

            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Success", result.Message, "OK");

                // IMPORTANT: Manually update the global state so the previous screen (Account Tab) 
                // updates its Image/Name immediately without needing a full API reload.
                _userAuthViewModel.ProfileDisplayName = DisplayName;

                // If we picked a new local file, we might not have the server URL yet unless we fetch it.
                // For a smooth UI, we can temporarily keep the local image, or trigger a background refresh.
                // For now, we will leave the image as-is or you can call InitializeAsync() to re-fetch from server.
                // await _userAuthViewModel.InitializeAsync(); 

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
    }
}