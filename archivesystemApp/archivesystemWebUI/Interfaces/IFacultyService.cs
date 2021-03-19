using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemWebUI.Services;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IFacultyService
    {
        FacultyServiceResult SaveFaculty(FacultyViewModel faculty);
    }
}