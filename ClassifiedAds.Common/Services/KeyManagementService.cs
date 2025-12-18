using ClassifiedAds.Common.Interfaces;

namespace ClassifiedAds.Common.Security
{
    public interface IKeyManagementService
    {
        Task<string> GetPublicKeyAsync(string memberId);
        Task<string> GetPrivateKeyAsync(string memberId);
        Task GenerateAndStoreKeysAsync(string memberId);
    }


    public class SimpleKeyManagementService : IKeyManagementService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly Dictionary<string, (string PublicKey, string PrivateKey)> _keys = new();

        public SimpleKeyManagementService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public Task<string> GetPublicKeyAsync(string memberId)
        {
            if (_keys.TryGetValue(memberId, out var pair))
                return Task.FromResult(pair.PublicKey);

            throw new InvalidOperationException("Keys not found for member");
        }

        public Task<string> GetPrivateKeyAsync(string memberId)
        {
            if (_keys.TryGetValue(memberId, out var pair))
                return Task.FromResult(pair.PrivateKey);

            throw new InvalidOperationException("Keys not found for member");
        }

        public Task GenerateAndStoreKeysAsync(string memberId)
        {
            var (pub, priv) = _encryptionService.GenerateKeyPair();
            _keys[memberId] = (pub, priv);
            return Task.CompletedTask;
        }
    }
}