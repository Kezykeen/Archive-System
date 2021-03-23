using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using archivesystemDomain.Entities;

namespace archivesystemDomain.Services
{
    public static class GetDesignation
    {
        public static string Value(Designation designation)
        {
            switch (designation)
            {
                case Designation.Alumni:
                    return "Alumni";
                case Designation.Staff:
                    return "Staff";
                case Designation.Student:
                    return "Student";
                default:
                    return null;
            }
        }
    }
}
