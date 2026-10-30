using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DataAccess;

public class GenericEfRepository<TModel> : IGenericRepository<TModel> where TModel : ModelBase
{
    protected readonly DbContext _context;

    public GenericEfRepository(DbContext context)
    {
        _context = context;
    }

    public List<TModel> GetList()
    {
        return _context.Set<TModel>().ToList();
    }

    public TModel? GetById(int id)
    {
        return _context.Set<TModel>().FirstOrDefault(m => m.Id == id);
    }

    public int Create(TModel model)
    {
        _context.Set<TModel>().Add(model);
        _context.SaveChanges();
        return model.Id;
    }

    public void Update(TModel model)
    {
        _context.Set<TModel>().Update(model);
        _context.SaveChanges();
    }

    public int Delete(int id)
    {
        var entity = _context.Set<TModel>().Find(id);
        if (entity == null)
        {
            return 0;
        }
        _context.Set<TModel>().Remove(entity);
        return _context.SaveChanges();
    }
}