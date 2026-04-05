using AutoMapper;
using Dotnet9.Models.DTO;

namespace Dotnet9.Models.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() { 
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Courses, CourseDto>().ReverseMap();
            //CreateMap<Mall, MallDto>().ReverseMap;
            CreateMap<Mall, MallDto>()
                .ForMember(dest => dest.Documents, opt => opt.Ignore());

            // ✅ MallDto → Mall (for POST/PUT — ignore Documents, handled manually)
            CreateMap<MallDto, Mall>()
                .ForMember(dest => dest.Documents, opt => opt.Ignore());

            CreateMap<MallDocument, MallDocumentDto>()
                .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => $"/api/malls/{src.MallId}/documents/{src.Id}/download"));

        }
    }
}
