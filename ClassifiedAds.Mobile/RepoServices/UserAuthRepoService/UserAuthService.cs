using ClassifiedAds.Mobile.Models;
using System.Text.RegularExpressions;

namespace ClassifiedAds.Mobile.RepoServices.UserAuthRepoService
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IUserAuthRepository _repository;
        private const string TokenKey = "auth_token";

        public UserAuthService(IUserAuthRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            // 1. Input Sanitization (Security Best Practice)
            // Prevent simple injection or buffer overflow attempts by trimming and checking length.
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;
            if (email.Length > 255 || password.Length > 255) return null;

            // Strip HTML tags if any (XSS prevention basic step)
            email = Regex.Replace(email, "<.*?>", string.Empty);

            // 2. Call Repository
            var user = await _repository.LoginAsync(email, password);

            // 3. Secure Storage
            if (user != null && !string.IsNullOrEmpty(user.Token))
            {
                // SecureStorage encrypts the token on the device (Keystore on Android)
                await SecureStorage.Default.SetAsync(TokenKey, user.Token);
            }

            return user;
        }

        public async Task LogoutAsync()
        {
            // 1. Get the current token
            var token = await SecureStorage.Default.GetAsync(TokenKey);

            // 2. Call the server-side logout (if we have a token)
            if (!string.IsNullOrEmpty(token))
            {
                await _repository.LogoutAsync(token);
            }

            // 3. Remove local token (This happens regardless of server success)
            SecureStorage.Default.Remove(TokenKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            return !string.IsNullOrEmpty(token);
        }
    }
}
