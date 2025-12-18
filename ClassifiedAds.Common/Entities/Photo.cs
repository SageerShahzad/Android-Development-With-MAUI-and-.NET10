using System.Text.Json.Serialization;

namespace ClassifiedAds.Common.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }
    public bool IsApproved { get; set; }

    public string MemberId { get; set; } = null!;

    [JsonIgnore]
    public Member Member { get; set; } = null!;

    public int? AdId { get; set; }


    [JsonIgnore]
    public Ad? Ad { get; set; }
}
