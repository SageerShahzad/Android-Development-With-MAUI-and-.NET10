using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Messages;

public class StatusChangedMessage
{
    public Guid EventId { get; }
    public AdStatusEnum Status { get; }

    public StatusChangedMessage(
        Guid id, 
        AdStatusEnum status)
    {
        EventId = id;
        Status = status;
    }
}
