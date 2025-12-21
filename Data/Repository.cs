using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cosmatics.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
        await SaveChangesAsync();
    }
    
    // Overload for deleting by ID if convenient (not in interface strictly but useful, using the interface pattern requires fetching first or stubbing)
    // To match interface that might not have Delete(id), we stick to Delete(entity) or we update Interface.
    // The previous interface definition had DeleteAsync(T entity). Let's check.
    // Interface had: Task DeleteAsync(T entity);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
