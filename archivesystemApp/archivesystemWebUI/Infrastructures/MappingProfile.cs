using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Infrastructures
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<EnrollViewModel, Employee>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
                )
                .ForMember(dest => dest.CreatedAt, opt => opt.UseValue(DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.UseValue(DateTime.Now));
            Mapper.CreateMap<EnrollViewModel, Employee>();
            Mapper.CreateMap<CreateAccessLevelViewModel, AccessLevel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.UseValue<DateTime>(DateTime.Now))
                .ForMember(m => m.UpdatedAt, opt => opt.UseValue<DateTime>(DateTime.Now));

            Mapper.CreateMap<EnrollViewModel, EmpUniqueProps>().ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
            );
        }
    }
}