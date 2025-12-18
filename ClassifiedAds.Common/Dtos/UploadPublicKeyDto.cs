namespace ClassifiedAds.Common.DTOs;

// For uploading public key
public class UploadPublicKeyDto
{
    public required string PublicKey { get; set; }  // Base64-encoded
}

// For fetching public key
public class PublicKeyDto
{
    public string PublicKey { get; set; } = string.Empty;
}