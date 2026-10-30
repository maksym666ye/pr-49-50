using PostService.CommonTypes;
using PostService.DataAccess;
using PostService.Models;

namespace PostService.BusinessLogic;

public class PostingService : IPostingService
{
    private readonly IPostingRepository _repository;

    public PostingService(IPostingRepository repository)
    {
        _repository = repository;
    }

    public Posting Create(Posting newPosting)
    {
        newPosting.CreatedAt = DateTime.UtcNow;

        float basePrice = newPosting.Weight * 20.0f;
        float deliveryFee = newPosting.DeliveryType == DeliveryType.Courier ? 50.0f : 0.0f;
        newPosting.Price = basePrice + deliveryFee;

        var generatedId = _repository.Create(newPosting);
        newPosting.Id = generatedId;
        return newPosting;
    }

    public List<Posting> GetAll() => _repository.GetList();

    public Posting? Find(int postingId) => _repository.GetById(postingId);

    public Posting? Update(Posting updated)
    {
        var existing = _repository.GetById(updated.Id);
        if (existing == null) return null;

        _repository.Update(updated);
        return _repository.GetById(updated.Id);
    }

    public bool Delete(int postingId)
    {
        var deletedCount = _repository.Delete(postingId);
        return deletedCount > 0;
    }
}