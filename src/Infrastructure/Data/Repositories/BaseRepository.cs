﻿using Microsoft.EntityFrameworkCore;
using SGS.TaskTracker.Core.Data;
using SGS.TaskTracker.Core.Interfaces;
using System.Linq.Expressions;

namespace SGS.TaskTracker.Infrastructure.Data.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly TaskTrackerContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(TaskTrackerContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public virtual void Update(T entity) => _dbSet.Update(entity);

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
