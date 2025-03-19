using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class RoomMappingProfile : Profile
    {
        public RoomMappingProfile()
        {
          

            CreateMap<RoomCreateDto, Room>()
                .ForMember(dest => dest.GroupMembers, opt => opt.MapFrom(src => src.GroupMembers));

            CreateMap<GroupMemberCreateDto, GroupMember>();
        }
    }
}
