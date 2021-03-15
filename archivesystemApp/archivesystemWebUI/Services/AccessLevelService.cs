using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace archivesystemWebUI.Services
{
    public class AccessLevelService : IAccessLevelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccessLevelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public IEnumerable<AccessLevel> GetAll()
        {
            return _unitOfWork.AccessLevelRepo.GetAll();
        }

        public async Task Create(AccessLevel newAccess)
        {
            _unitOfWork.AccessLevelRepo.Add(newAccess);
            await _unitOfWork.SaveAsync();
        }

        public AccessLevel GetById(int id)
        {
            return _unitOfWork.AccessLevelRepo.Get(id);
        }

        public async Task Update(AccessLevel accessLevel)
        {
            _unitOfWork.AccessLevelRepo.EditDetails(accessLevel);
            await _unitOfWork.SaveAsync();
        }

        public bool CheckLevel(string Level)
        {
            var checkLevel = _unitOfWork.AccessLevelRepo.GetByLevel(Level);
           return checkLevel == null;
        }
    }
}