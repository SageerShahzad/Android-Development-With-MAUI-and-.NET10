using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Common.Entities.Encryption
{
    public interface IFileCryptoService
    {
        Task<(string TempFilePath, string OriginalDirectoryName)> DecryptDirectoryAsync(IFormFile encryptedFile, string password);
        Task<(string TempFilePath, string OriginalFileName)> DecryptFileAsync(IFormFile encryptedFile, string password);
        Task<string> EncryptDirectoryAsync(IFormFile directoryZip, string password);
        Task<string> EncryptFileAsync(IFormFile file, string password);
    }
}
