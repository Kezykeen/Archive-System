using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IStaffRepository
    {
        bool EmailExists(string email);
    }
}
