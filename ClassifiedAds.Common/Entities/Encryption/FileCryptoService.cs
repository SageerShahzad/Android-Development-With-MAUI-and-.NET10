using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ClassifiedAds.Common.Entities.Encryption
{
    public class FileCryptoService : IFileCryptoService
    {
        private const int MaxPaddingSize = 10 * 1024 * 1024; // 10 MB
        private readonly ILogger<FileCryptoService> _logger;

        public FileCryptoService(ILogger<FileCryptoService> logger)
        {
            _logger = logger;
        }

        public async Task<string> EncryptFileAsync(IFormFile file, string password)
        {
            string tempOutput = Path.GetTempFileName();
            string extension = Path.GetExtension(file.FileName);

            using (var inputStream = file.OpenReadStream())
            using (var outputStream = new FileStream(tempOutput, FileMode.Create, FileAccess.Write))
            {
                await EncryptStreamAsync(inputStream, outputStream, password, extension);
            }

            return tempOutput;
        }

        public async Task<string> EncryptDirectoryAsync(IFormFile directoryZip, string password)
        {
            return await EncryptFileAsync(directoryZip, password);
        }

        public async Task<(string TempFilePath, string OriginalFileName)> DecryptFileAsync(IFormFile encryptedFile, string password)
        {
            string tempOutput = Path.GetTempFileName();

            using (var inputStream = encryptedFile.OpenReadStream())
            using (var outputStream = new FileStream(tempOutput, FileMode.Create, FileAccess.Write))
            {
                string originalExtension = await DecryptStreamAsync(inputStream, outputStream, password);
                string originalFileName = $"{Path.GetFileNameWithoutExtension(encryptedFile.FileName)}{originalExtension}";
                return (tempOutput, originalFileName);
            }
        }

        public async Task<(string TempFilePath, string OriginalDirectoryName)> DecryptDirectoryAsync(IFormFile encryptedFile, string password)
        {
            var result = await DecryptFileAsync(encryptedFile, password);
            return (result.TempFilePath, Path.GetFileNameWithoutExtension(result.OriginalFileName));
        }

        private async Task EncryptStreamAsync(Stream input, Stream output, string password, string extension)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            await output.WriteAsync(salt, 0, salt.Length);

            var keyGenerator = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] key = keyGenerator.GetBytes(32);
            byte[] iv = keyGenerator.GetBytes(16);

            int paddingSize = RandomNumberGenerator.GetInt32(1, MaxPaddingSize);
            byte[] padding = new byte[paddingSize];
            RandomNumberGenerator.Fill(padding);

            byte[] paddingSizeBytes = BitConverter.GetBytes(paddingSize);
            await output.WriteAsync(paddingSizeBytes, 0, 4);

            byte[] extensionBytes = Encoding.UTF8.GetBytes(extension);
            await output.WriteAsync(new[] { (byte)extensionBytes.Length }, 0, 1);
            await output.WriteAsync(extensionBytes, 0, extensionBytes.Length);

            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var cryptoStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(padding, 0, padding.Length);
                await input.CopyToAsync(cryptoStream);
            }
        }

        private async Task<string> DecryptStreamAsync(Stream input, Stream output, string password)
        {
            byte[] salt = new byte[16];
            await input.ReadExactlyAsync(salt, 0, 16);

            var keyGenerator = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] key = keyGenerator.GetBytes(32);
            byte[] iv = keyGenerator.GetBytes(16);

            byte[] paddingSizeBytes = new byte[4];
            await input.ReadExactlyAsync(paddingSizeBytes, 0, 4);
            int paddingSize = BitConverter.ToInt32(paddingSizeBytes);

            int extensionLength = input.ReadByte();
            if (extensionLength == -1) throw new InvalidDataException("Invalid extension length");

            byte[] extensionBytes = new byte[extensionLength];
            await input.ReadExactlyAsync(extensionBytes, 0, extensionLength);
            string originalExtension = Encoding.UTF8.GetString(extensionBytes);

            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(key, iv))
            using (var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
            {
                // Skip padding bytes
                byte[] buffer = new byte[4096];
                int bytesToSkip = paddingSize;
                while (bytesToSkip > 0)
                {
                    int bytesToRead = Math.Min(buffer.Length, bytesToSkip);
                    int read = await cryptoStream.ReadAsync(buffer, 0, bytesToRead);
                    if (read == 0) break;
                    bytesToSkip -= read;
                }

                // Copy remaining data to output
                await cryptoStream.CopyToAsync(output);
            }

            return originalExtension;
        }
    }
}