using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Common.Dtos.Encryption
{
    public class DecryptionRequest
    {
        public IFormFile EncryptedFile { get; set; }
        public string Password { get; set; }
        public string OriginalExtension { get; set; }
    }
}
