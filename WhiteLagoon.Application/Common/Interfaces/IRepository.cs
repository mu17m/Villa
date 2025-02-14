using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        public void Add(T entity);
        public void Remove(T entity);
        bool Any(Expression<Func<T, bool>> filter);
    }
}
