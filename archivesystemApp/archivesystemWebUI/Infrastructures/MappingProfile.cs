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
            Mapper.CreateMap<EnrollViewModel, Employee>();
            Mapper.CreateMap<CreateAccessLevelViewModel, AccessLevel>().ForMember(dest => dest.CreatedAt, opt => opt.UseValue<DateTime>(DateTime.Now)).ForMember(m => m.UpdatedAt, opt => opt.UseValue<DateTime>(DateTime.Now));

        }
    }
}