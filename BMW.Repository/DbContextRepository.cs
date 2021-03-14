using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;

namespace BMW.Repository
{
    public class DbContextRepository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _objectSet;
        private DbContext _context;
        private bool _disposed = false;

        public DbContextRepository(DbContext context)
        {
            _objectSet = context.Set<T>();
            _context = context;
        }

        public IQueryable<T> AsQueryable()
        {
            return _objectSet;
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            entities.ToList().ForEach(Delete);
        }

        public T Get(int id)
        {
            return _objectSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _objectSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.Where(where);
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return _objectSet.Single(where);
        }

        public T First(Expression<Func<T, bool>> where)
        {
            return _objectSet.First(where);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _objectSet.Attach(entity);
            _objectSet.Remove(entity);
        }

        public void Add(T entity)
        {
            _objectSet.Add(entity);
        }

        public void Update(T entity)
        {
            _objectSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context as IUnitOfWork;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                { // 释放托管资源
                    System.Diagnostics.Trace.WriteLine("context dispose");
                    _context.Dispose();
                }
                // 释放非托管资源
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DbContextRepository()
        {
            Dispose(false);
        }
    }
}
