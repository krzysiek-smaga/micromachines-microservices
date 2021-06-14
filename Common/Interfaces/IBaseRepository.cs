using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IList<T> GetAll();
        T GetSingle(Func<T, bool> condition);
        T Add(T entity);
        T Edit(T entity);
        void Delete(T entity);
        void Save();
    }
}