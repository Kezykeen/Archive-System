using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IUnitOfWork
    {
        IFileRepo FileRepo { get; }
        ITicketRepo TicketRepo { get; }
        IApplicationRepo ApplicationRepo { get;  }


        int Save();
        Task<int> SaveAsync();

    }
}