using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            CreateMap<ProjectPostDto, Project>()
                .ForMember(dest => dest.ProjectMembers, opt => opt.MapFrom((src, dest) =>
                    src.MemberIds.Select(id => new ProjectMember { AccountId = id }).ToList()));
            CreateMap<ProjectGetDto, Project>().ReverseMap();
            CreateMap<ProjectUpdateDto, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
