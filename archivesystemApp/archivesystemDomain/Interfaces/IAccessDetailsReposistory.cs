﻿using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IAccessDetailsRepository : IRepository<AccessDetails>
    {
        void EditDetails(AccessDetails accessDetails);
    }
}
