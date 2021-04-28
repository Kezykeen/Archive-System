﻿using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class AccessDetailsRepository: Repository<AccessDetail>, IAccessDetailsRepository
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
        public void EditDetails(AccessDetail accessDetails) => _context.Entry(accessDetails).State = EntityState.Modified;

        /// <summary>
        /// This method retrieves an "AccessDetails" object using the "EmployeeId" parameter of the table
        /// </summary>
        /// <param name="employeeId">Integer</param>
        /// <returns>AccessDetails</returns>
        public AccessDetail GetByAppUserId(int appUserId) => _context.AccessDetails.Where(m => m.AppUserId == appUserId).FirstOrDefault();

        public IEnumerable<AccessDetail> GetAccessDetails() => _context.AccessDetails.Include(m => m.AppUser).Include(m => m.AccessLevel).ToList();
    }


}