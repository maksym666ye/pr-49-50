using PostService.Models;

namespace PostService.DataAccess;

public interface IGenericRepository<TModel> where TModel : ModelBase
{
    List<TModel> GetList();
    TModel? GetById(int id);
    int Create(TModel model);
    void Update(TModel model);
    int Delete(int id);
}