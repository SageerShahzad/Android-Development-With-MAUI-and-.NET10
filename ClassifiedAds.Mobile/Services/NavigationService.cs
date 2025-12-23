using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.Services;

namespace GloboTicket.Admin.Mobile.Services;

public class NavigationService : INavigationService
{
    public async Task GoToEventDetail(Guid id)
    {
        var parameters = new Dictionary<string, object> { {"EventId", id}};
        await Shell.Current.GoToAsync("event", parameters);
    }

    public Task GoToAddEvent()
        => Shell.Current.GoToAsync("event/add");

    public async Task GoToEditEvent(EventModel detailModel)
    {
        var navigationParameter = new ShellNavigationQueryParameters
        {
            { "Event", detailModel }
        };

        await Shell.Current.GoToAsync("event/edit", navigationParameter);
    }

    public Task GoToOverview()
        => Shell.Current.GoToAsync("//overview");

    public Task GoBack()
        => Shell.Current.GoToAsync("..");
}
