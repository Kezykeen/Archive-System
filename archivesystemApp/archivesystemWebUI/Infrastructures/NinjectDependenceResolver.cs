using archivesystemWebUI.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using archivesystemDomain;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Repository;
using Ninject.Web.Common;
using archivesystemWebUI.Services;
using archivesystemWebUI.Interfaces;

namespace archivesystemWebUI.Infrastructures
{
    public class NinjectDependenceResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependenceResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind(typeof(ApplicationDbContext)).ToSelf().InRequestScope();
            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();
            kernel.Bind<IDeptRepository>().To<DeptRepository>();
            kernel.Bind<IAccessDetailsRepository>().To<AccessDetailsRepository>();
            kernel.Bind<IAccessLevelRepository>().To<AccessLevelRepository>();
            kernel.Bind<ITokenRepo>().To<TokenRepository>();
            kernel.Bind<IRoleService>().To<RoleService>();
            kernel.Bind<ITokenGenerator>().To<TokenGenerator>();
            kernel.Bind<IEmailSender>().To<EmailSender>();
            kernel.Bind<IFolderRepo>().To<FolderRepo>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IFacultyRepository>().To<FacultyRepository>();
            kernel.Bind<IFileMetaRepo>().To<FileMetaRepo>();
            kernel.Bind<IAccessLevelService>().To<AccessLevelService>();
            kernel.Bind<IUserAccessService>().To<UserAccessService>();
            kernel.Bind<IFileRepo>().To<FileRepo>();
        }
    }
}