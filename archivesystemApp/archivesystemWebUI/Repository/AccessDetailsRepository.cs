using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class AccessDetailsRepository: Repository<AccessDetails>, IAccessDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public AccessDetailsRepository(ApplicationDbContext context)
        : base(context)
        {
            _context = context;
        }


        /// <summary>
        /// This method accepts an "AcceptDetails" object and modifies an existing data in the AccessDetails table.
        /// </summary>
        /// <param name="accessDetails"> AccessDetails</param>
        public void EditDetails(AccessDetails accessDetails)
        {
            _context.Entry(accessDetails).State = EntityState.Modified;
        }

        /// <summary>
        /// This method retrieves an "AccessDetails" object using the "EmployeeId" parameter of the table
        /// </summary>
        /// <param name="employeeId">Integer</param>
        /// <returns>AccessDetails</returns>
        public AccessDetails GetByEmployeeId(int employeeId)
        {
            return _context.AccessDetails.Where(m => m.EmployeeId == employeeId).FirstOrDefault();
        }
    }


}