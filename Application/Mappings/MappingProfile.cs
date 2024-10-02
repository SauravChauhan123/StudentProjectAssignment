using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.StudentNames, opt => opt.MapFrom(src => src.Students.Select(s => s.Name)))
            .ReverseMap()
            .ForMember(dest => dest.Students, opt => opt.Ignore()); 
            CreateMap<Student, StudentDtos>()
            .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Projects)); 
            CreateMap<Student, StudentDto>().ReverseMap();
        }
    }
}
