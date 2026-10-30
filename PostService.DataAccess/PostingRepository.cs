using PostService.Models;

namespace PostService.DataAccess;

public class PostingRepository : GenericEfRepository<Posting>, IPostingRepository
{
    public PostingRepository(PostingDbContext context) : base(context)
    {
    }
}