using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Infrastructures
{
    public  class EmployeeValidator
    {
       
        public static (string msg, bool exists) Validate(IUnitOfWork unitOfWork, EmpUniqueProps uniqueProps)
        {
            if (unitOfWork.EmployeeRepo.NameExists(uniqueProps.Name))
            {
                return ("Name Already taken", true);
            }

            if (unitOfWork.EmployeeRepo.EmailExists(uniqueProps.Email))
            {
                return ("Email Already Exists", true);
            }

            if (unitOfWork.EmployeeRepo.StaffIdExists(uniqueProps.StaffId))
            {
               return ("Staff Id Already Exists", true);
               
            }
            if (unitOfWork.EmployeeRepo.StaffIdExists(uniqueProps.Phone))
            {
                return ("Phone Number Already Exists", true);

            }

            return ("", false);
        }
    }
}