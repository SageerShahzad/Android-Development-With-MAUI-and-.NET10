namespace ClassifiedAds.Common.Entities.Encryption
{
    public class FileEncryptionInfo
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string EncryptedFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSize { get; set; }
    }
}
