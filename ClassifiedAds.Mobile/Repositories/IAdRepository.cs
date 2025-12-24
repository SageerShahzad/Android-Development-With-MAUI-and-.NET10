using ClassifiedAds.Mobile.Models;



namespace ClassifiedAds.Mobile.Repositories;



public interface IAdRepository

{

    // We only need this one for now to display the detail page

    Task<AdDTO?> GetAd(int id);

}