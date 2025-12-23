using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Services;

public interface IAdService
{
    Task<List<EventModel>> GetEvents();
    Task<EventModel?> GetEvent(Guid id);
    Task<bool> UpdateStatus(Guid id, EventStatusModel status);
    Task<bool> CreateEvent(EventModel model);
    Task<bool> EditEvent(EventModel model);
    Task<bool> DeleteEvent(Guid id);
}
