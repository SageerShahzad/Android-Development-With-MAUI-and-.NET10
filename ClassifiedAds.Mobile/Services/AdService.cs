using ClassifiedAds.Mobile.Models;

using ClassifiedAds.Mobile.Repositories;



namespace ClassifiedAds.Mobile.Services;



public class AdService : IAdService

{

    private readonly IAdRepository _repository;



    public AdService(IAdRepository repository)

    {

        _repository = repository;

    }



    public Task<AdDTO?> GetAdById(int id)

    {

        return _repository.GetAd(id);

    }

}