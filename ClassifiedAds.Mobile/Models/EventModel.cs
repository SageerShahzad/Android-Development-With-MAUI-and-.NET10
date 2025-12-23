namespace ClassifiedAds.Mobile.Models;

public class EventModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Name { get; set; } = default!;
    public double Price { get; set; }
    public DateTime Date { get; set; }
    public List<string> Artists { get; set; } = default!;
    public string? Description { get; set; }
    public EventStatusModel Status { get; set; }
    public CategoryModel Category { get; set; } = default!;
}
