using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Infrastructures
{
    public  class UserValidator
    {
       
        public static (string msg, bool exists) Validate(IUnitOfWork unitOfWork, EmpUniqueProps uniqueProps, int? userId = null)
        {
            if (unitOfWork.UserRepo.NameExists(uniqueProps.Name, userId ))
            {
                return ("Name Already taken", true);
            }

            if (unitOfWork.UserRepo.EmailExists(uniqueProps.Email, userId))
            {
                return ("Email Already Exists", true);
            }

            if (unitOfWork.UserRepo.TagIdExists(uniqueProps.StaffId, userId))
            {
               return ("Already Exists", true);
               
            }
            return unitOfWork.UserRepo.PhoneExists(uniqueProps.Phone, userId) ? ("Phone Number Already Exists", true) : ("", false);
        }
    }
}