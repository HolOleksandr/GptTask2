using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        T GetRepository<T>();
        Task SaveAsync();
    }
}
