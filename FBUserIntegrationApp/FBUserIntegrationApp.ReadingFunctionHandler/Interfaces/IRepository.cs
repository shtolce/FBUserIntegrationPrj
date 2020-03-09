using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMCDataService.DAL.Interfaces
{
    public interface IRepositorySQL1C<T> where T:class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset);
        T GetById(string id);
        bool Create(T obj);
        void Update(T obj);
        void Delete(int id);
    }
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset);
        T GetById(string id);
        bool Create(T obj);
        void Update(T obj);
        void Delete(int id);
    }


}
