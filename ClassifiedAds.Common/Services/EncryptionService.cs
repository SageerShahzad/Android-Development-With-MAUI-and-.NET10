using System.Security.Cryptography;
using System.Text.Json;

namespace ClassifiedAds.Common.Security
{
    public interface IEncryptionService
    {
        (string EncryptedData, string PublicKey) Encrypt(string plainText, string recipientPublicKey);
        string EncryptForBoth(string plainText, string senderPublicKey, string recipientPublicKey);
        string Decrypt(string encryptedData, string privateKey);
        string DecryptForUser(string encryptedPayloadJson, string privateKey, bool isSender);
        (string PublicKey, string PrivateKey) GenerateKeyPair();
    }

    public class E2eeEncryptionService : IEncryptionService
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public (string EncryptedData, string PublicKey) Encrypt(string plainText, string recipientPublicKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return (string.Empty, string.Empty);

            try
            {
                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();

                var encryptedBytes = EncryptWithAes(plainText, aes.Key, aes.IV);
                var encryptedKey = EncryptKeyWithRsa(aes.Key, recipientPublicKey);

                var payload = new EncryptedPayload
                {
                    Data = Convert.ToBase64String(encryptedBytes),
                    Key = Convert.ToBase64String(encryptedKey),
                    IV = Convert.ToBase64String(aes.IV)
                };

                return (JsonSerializer.Serialize(payload, _jsonOptions), recipientPublicKey);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to encrypt message", ex);
            }
        }

        public string Decrypt(string encryptedData, string privateKey)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return string.Empty;

            try
            {
                var payload = JsonSerializer.Deserialize<EncryptedPayload>(encryptedData, _jsonOptions);
                if (payload == null)
                    throw new InvalidOperationException("Invalid encrypted payload");

                // Try to decrypt using the standard Key property first (one-way)
                if (!string.IsNullOrEmpty(payload.Key))
                {
                    var aesKey = DecryptKeyWithRsa(Convert.FromBase64String(payload.Key), privateKey);
                    return DecryptWithAes(
                        Convert.FromBase64String(payload.Data),
                        aesKey,
                        Convert.FromBase64String(payload.IV)
                    );
                }

                // If no standard Key, it might be a two-way encrypted message
                // For two-way messages, you need to know if you're the sender or recipient
                throw new InvalidOperationException("This is a two-way encrypted message. Use DecryptForUser instead.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt message", ex);
            }
        }

        public string DecryptForUser(string encryptedPayloadJson, string privateKey, bool isSender)
        {
            if (string.IsNullOrEmpty(encryptedPayloadJson))
            {
                Console.WriteLine("Encrypted payload JSON is null or empty");
                throw new InvalidOperationException("Encrypted payload is empty");
            }

            try
            {
                Console.WriteLine($"Attempting to decrypt for {(isSender ? "sender" : "recipient")}");
                Console.WriteLine($"Encrypted payload length: {encryptedPayloadJson.Length}");

                var payload = JsonSerializer.Deserialize<EncryptedPayload>(encryptedPayloadJson, _jsonOptions);
                if (payload == null)
                {
                    Console.WriteLine("Failed to deserialize encrypted payload");
                    throw new InvalidOperationException("Invalid encrypted payload");
                }

                Console.WriteLine($"Payload has Data: {!string.IsNullOrEmpty(payload.Data)}");
                Console.WriteLine($"Payload has IV: {!string.IsNullOrEmpty(payload.IV)}");
                Console.WriteLine($"Payload has KeyForSender: {!string.IsNullOrEmpty(payload.KeyForSender)}");
                Console.WriteLine($"Payload has KeyForRecipient: {!string.IsNullOrEmpty(payload.KeyForRecipient)}");
                Console.WriteLine($"Payload has Key: {!string.IsNullOrEmpty(payload.Key)}");

                string keyBase64;

                if (isSender)
                {
                    // Try KeyForSender first, then fall back to Key
                    if (!string.IsNullOrEmpty(payload.KeyForSender))
                    {
                        keyBase64 = payload.KeyForSender;
                        Console.WriteLine("Using KeyForSender for decryption");
                    }
                    else if (!string.IsNullOrEmpty(payload.Key))
                    {
                        keyBase64 = payload.Key;
                        Console.WriteLine("Using Key (fallback) for sender decryption");
                    }
                    else
                    {
                        Console.WriteLine("No key found for sender");
                        throw new InvalidOperationException("No key for sender");
                    }
                }
                else
                {
                    // Try KeyForRecipient first, then fall back to Key
                    if (!string.IsNullOrEmpty(payload.KeyForRecipient))
                    {
                        keyBase64 = payload.KeyForRecipient;
                        Console.WriteLine("Using KeyForRecipient for decryption");
                    }
                    else if (!string.IsNullOrEmpty(payload.Key))
                    {
                        keyBase64 = payload.Key;
                        Console.WriteLine("Using Key (fallback) for recipient decryption");
                    }
                    else
                    {
                        Console.WriteLine("No key found for recipient");
                        throw new InvalidOperationException("No key for recipient");
                    }
                }

                Console.WriteLine($"Key length: {keyBase64?.Length ?? 0}");
                Console.WriteLine($"Data length: {payload.Data?.Length ?? 0}");
                Console.WriteLine($"IV length: {payload.IV?.Length ?? 0}");

                // Decrypt the AES key with RSA private key
                var aesKey = DecryptKeyWithRsa(Convert.FromBase64String(keyBase64), privateKey);
                Console.WriteLine("AES key decrypted successfully");

                // Decrypt the message with AES
                var decrypted = DecryptWithAes(
                    Convert.FromBase64String(payload.Data),
                    aesKey,
                    Convert.FromBase64String(payload.IV)
                );

                Console.WriteLine($"Message decrypted successfully, length: {decrypted.Length}");
                return decrypted;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException during decryption: {ex.Message}");
                throw new InvalidOperationException("Invalid base64 format in encrypted payload", ex);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"CryptographicException during decryption: {ex.Message}");
                throw new InvalidOperationException("Decryption failed - possibly wrong key", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException during decryption: {ex.Message}");
                Console.WriteLine($"JSON content: {encryptedPayloadJson}");
                throw new InvalidOperationException("Invalid JSON in encrypted payload", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during decryption: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                throw new InvalidOperationException("Failed to decrypt message", ex);
            }
        }

        public (string PublicKey, string PrivateKey) GenerateKeyPair()
        {
            using var rsa = RSA.Create(2048);
            return (
                Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                Convert.ToBase64String(rsa.ExportPkcs8PrivateKey())
            );
        }

        public string EncryptForBoth(string plainText, string senderPublicKey, string recipientPublicKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            Console.WriteLine($"Encrypting message of length: {plainText.Length}");
            Console.WriteLine($"Using sender public key (length: {senderPublicKey?.Length ?? 0})");
            Console.WriteLine($"Using recipient public key (length: {recipientPublicKey?.Length ?? 0})");

            var encryptedData = EncryptWithAes(plainText, aes.Key, aes.IV);
            var keyForSender = EncryptKeyWithRsa(aes.Key, senderPublicKey);
            var keyForRecipient = EncryptKeyWithRsa(aes.Key, recipientPublicKey);

            var payload = new EncryptedPayload
            {
                Data = Convert.ToBase64String(encryptedData),
                KeyForSender = Convert.ToBase64String(keyForSender),
                KeyForRecipient = Convert.ToBase64String(keyForRecipient),
                IV = Convert.ToBase64String(aes.IV)
            };

            var result = JsonSerializer.Serialize(payload, _jsonOptions);
            Console.WriteLine($"Encrypted payload JSON length: {result.Length}");

            return result;
        }

        private byte[] EncryptWithAes(string plainText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return ms.ToArray();
        }

        private string DecryptWithAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        private byte[] EncryptKeyWithRsa(byte[] key, string publicKeyBase64)
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);
            return rsa.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        }

        private byte[] DecryptKeyWithRsa(byte[] encryptedKey, string privateKeyBase64)
        {
            using var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
            return rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
        }

        private class EncryptedPayload
        {
            public string Data { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty; // For one-way encryption
            public string KeyForSender { get; set; } = string.Empty; // For two-way encryption
            public string KeyForRecipient { get; set; } = string.Empty; // For two-way encryption
            public string IV { get; set; } = string.Empty;
        }
    }
}