using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Common.Entities
{
    public class DevicePublicKey
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DeviceId { get; set; } = default!;
        public string MemberId { get; set; } = default!;
        public string PublicKeyBase64 { get; set; } = default!;
        public string? KeyType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public Member Member { get; set; } = default!;
    }
}
