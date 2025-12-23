using ClassifiedAds.Mobile.Models;

namespace GloboTicket.Admin.Mobile.Repositories;

public interface IAdRepository
{
    Task<List<EventModel>> GetEvents();
    Task<EventModel?> GetEvent(Guid id);
    Task<bool> UpdateStatus(Guid id, EventStatusModel status);
    Task<bool> CreateEvent(EventModel model);
    Task<bool> EditEvent(EventModel model);
    Task<bool> DeleteEvent(Guid id);
}
