using AutoMapper;
using Dotnet9.Models.DTO;

namespace Dotnet9.Models.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() { 
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Courses, CourseDto>().ReverseMap();
        }
    }
}
