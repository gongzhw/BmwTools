using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BMW.Repository
{
	public interface IRepository {
		IUnitOfWork UnitOfWork {
			get;
		}
	}

	public interface IRepository<T> : IRepository, IDisposable where T: class {
		IQueryable<T> AsQueryable();
	    void DeleteAll(IEnumerable<T> entities);
		IEnumerable<T> GetAll();
		IEnumerable<T> Find(Expression<Func<T, bool>> where);

		T Single(Expression<Func<T, bool>> where);
		T First(Expression<Func<T, bool>> where);
        T FirstOrDefault(Expression<Func<T, bool>> where);

	    T Get(int id);
		void Delete(T entity);
		void Add(T entity);
		void Update(T entity);
	}
}
