using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Common.Dtos
{
    public class RegisterKeyDto
    {
        public required string DeviceId { get; set; }
        public required string PublicKeyBase64 { get; set; }
        public string? KeyType { get; set; }                    // e.g. "RSA-OAEP-2048" or "ECDH-P256"
    }
}
