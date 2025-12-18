namespace ClassifiedAds.Common.Dtos.Encryption
{
    public class EncryptionResult
    {
        public string EncryptedFilePath { get; set; }
        public long OriginalSize { get; set; }
        public long EncryptedSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
