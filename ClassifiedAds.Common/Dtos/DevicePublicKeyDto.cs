using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Common.Dtos
{
    public class DevicePublicKeyDto
    {
        public required string DeviceId { get; set; }
        public required string PublicKeyBase64 { get; set; }
        public string? KeyType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
