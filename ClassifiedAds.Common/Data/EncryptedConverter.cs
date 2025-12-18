using ClassifiedAds.Common.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClassifiedAds.Common.Data;

public class EncryptedConverter : ValueConverter<string, string>
{
    public EncryptedConverter()
        : base(
            v => EncryptionHelper.Encrypt(v), // Going to DB: Encrypt
            v => EncryptionHelper.Decrypt(v), // Coming from DB: Decrypt
            new ConverterMappingHints(size: null)) // Allow any size
    {
    }
}