using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.DataLayers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using archivesystemWebUI.Models.FolderViewModels;



namespace archivesystemWebUI.Infrastructures
{
    public class MappingProfile : Profile
    {
        private HttpContext Context => HttpContext.Current;
        private ApplicationUserManager UserManager => Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private ApplicationRoleManager RoleManager => Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        public MappingProfile()
        {


            Mapper.CreateMap<EnrollViewModel, Employee>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
                )
                .ForMember(dest => dest.CreatedAt, opt => opt.UseValue(DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.UseValue(DateTime.Now));


            Mapper.CreateMap<EnrollViewModel, EmpUniqueProps>().ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
            );

            Mapper.CreateMap<UpdateEmployeeVM, EmpUniqueProps>().ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
            );
            Mapper.CreateMap<UpdateEmployeeVM, Employee>().ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
            );

            Mapper.CreateMap<Employee, UserDataView>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Appointed, opt => opt.MapFrom(src => src.Appointed.ToString("dd MMM, yy")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd MMM, yy")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("dd MMM, yy")))
                .ForMember(dest => dest.Role, opt =>
                    {
                        opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.UserId));
                        opt.MapFrom(src => UserManager.GetRoles(src.UserId).FirstOrDefault());
                    })
                .ForMember(dest => dest.Gender, opt =>
                {
                    opt.MapFrom(src => src.Gender == Gender.Male ? "Male": "Female");
                })
                .ForMember(dest => dest.Completed, opt =>
                {
                    opt.MapFrom(src => src.Completed ? "Completed" : "Incomplete");
                })
               ;

            Mapper.CreateMap<Employee, UpdateEmployeeVM>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom( src => src.Name.Split().First()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom( src => src.Name.Split().Last()))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom( src => UserManager.FindById(src.UserId).Roles.SingleOrDefault().RoleId))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom( src => RoleManager.Roles.ToList()))
                ;
            Mapper.CreateMap<CreateAccessLevelViewModel, AccessLevel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.UseValue<DateTime>(DateTime.Now))
                .ForMember(m => m.UpdatedAt, opt => opt.UseValue<DateTime>(DateTime.Now));

           
            Mapper.CreateMap<EnrollViewModel, Employee>();
            Mapper.CreateMap<Department, DepartmentViewModel>().ReverseMap();
            Mapper.CreateMap<Faculty, FacultyViewModel>().ReverseMap();
            Mapper.CreateMap<CreateAccessLevelViewModel, AccessLevel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.UseValue<DateTime>(DateTime.Now))
                .ForMember(m => m.UpdatedAt, opt => opt.UseValue<DateTime>(DateTime.Now));
            Mapper.CreateMap<FolderPageViewModel, Folder>().ReverseMap();

            Mapper.CreateMap<DepartmentViewModel,Folder>()
                .ForMember(dest => dest.Id ,opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.UseValue(DateTime.Now));
            Mapper.CreateMap<FacultyViewModel, Folder>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt, opt => opt.UseValue(DateTime.Now))
               .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src=> src.Id));

            Mapper.CreateMap<Folder, FolderPageViewModel>()
                .ForMember(x=> x.DirectChildren,opt=> opt.MapFrom(src=> src.Subfolders));


        }
    }
}