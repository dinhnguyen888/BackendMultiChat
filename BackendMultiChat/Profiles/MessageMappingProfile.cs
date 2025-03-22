using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<RoomMessage, RMGetDto>();


            CreateMap<RMPostDto, RoomMessage>()
                .ForMember(dest => dest.SentDateTime, opt => opt.MapFrom(src => DateTime.UtcNow)) 
               ;
        }
    }
}
