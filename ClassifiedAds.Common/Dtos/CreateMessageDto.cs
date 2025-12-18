using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Common.DTOs;

public class CreateMessageDto
{
    public required string RecipientId { get; set; }
    public string? Content { get; set; }

    // This allows the API to receive the file
    public IFormFile? File { get; set; }
}
