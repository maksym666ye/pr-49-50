using PostService.Models;

namespace PostService.BusinessLogic;

public interface IPostingService 
{ 
    Posting Create(Posting newPosting); 
    Posting? Update(Posting posting); 
    List<Posting> GetAll(); 
    Posting? Find(int postingId); 
    bool Delete(int postingId); 
}