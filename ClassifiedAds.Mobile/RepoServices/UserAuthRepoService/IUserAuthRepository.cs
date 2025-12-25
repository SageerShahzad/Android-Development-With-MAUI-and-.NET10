using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.UserAuthRepoService
{
    public interface IUserAuthRepository
    {
        Task<UserDto?> LoginAsync(string email, string password);

        // ADD THIS: Accepts the token so we can send it in the Authorization header
        Task LogoutAsync(string token);
    }
}