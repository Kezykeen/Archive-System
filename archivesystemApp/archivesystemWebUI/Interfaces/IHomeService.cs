using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IHomeService
    {
        CardsViewModel GetAllClasses();

    }
}
