using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Common.Dtos.Encryption
{
    public class EncryptionRequest
    {
        public IFormFile File { get; set; }
        public string Password { get; set; }
        public bool SecureDelete { get; set; } = false;
    }
}
