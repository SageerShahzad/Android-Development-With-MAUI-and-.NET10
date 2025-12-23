using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Services;

public interface INavigationService
{
    Task GoToEventDetail(Guid selectedEventId);
    Task GoToAddEvent();
    Task GoToEditEvent(EventModel detailModel);
    Task GoToOverview();
    Task GoBack();
}
