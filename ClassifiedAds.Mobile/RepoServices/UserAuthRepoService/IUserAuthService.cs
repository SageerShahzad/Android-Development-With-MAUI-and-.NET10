using ClassifiedAds.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Mobile.RepoServices.UserAuthRepoService
{
    public interface IUserAuthService
    {
        Task<UserDto?> LoginAsync(string email, string password);
        Task LogoutAsync();

        // ADD THIS LINE
        Task<string?> GetTokenAsync();

        Task<bool> IsAuthenticatedAsync();
    }
}
