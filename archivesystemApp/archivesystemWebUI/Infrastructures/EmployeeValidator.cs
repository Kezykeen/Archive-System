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
       
        public static (string msg, bool exists) Validate(IUnitOfWork unitOfWork, EmpUniqueProps uniqueProps, int? userId = null)
        {
            if (unitOfWork.EmployeeRepo.NameExists(uniqueProps.Name, userId ))
            {
                return ("Name Already taken", true);
            }

            if (unitOfWork.EmployeeRepo.EmailExists(uniqueProps.Email, userId))
            {
                return ("Email Already Exists", true);
            }

            if (unitOfWork.EmployeeRepo.StaffIdExists(uniqueProps.StaffId, userId))
            {
               return ("Staff Id Already Exists", true);
               
            }
            if (unitOfWork.EmployeeRepo.PhoneExists(uniqueProps.Phone, userId))
            {
                return ("Phone Number Already Exists", true);

            }

            return ("", false);
        }
    }
}