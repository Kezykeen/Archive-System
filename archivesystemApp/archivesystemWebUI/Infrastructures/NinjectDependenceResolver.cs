using archivesystemWebUI.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using archivesystemDomain;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Repository;

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
            kernel.Bind(typeof(ApplicationDbContext)).ToSelf();
            kernel.Bind<IStaffRepository>().To<StaffRepository>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
        }
    }
}