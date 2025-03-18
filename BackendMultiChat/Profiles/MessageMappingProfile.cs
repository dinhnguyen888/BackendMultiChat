using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<Message, MessageGetDto>();


            CreateMap<MessagePostDto, Message>()
                .ForMember(dest => dest.SentDateTime, opt => opt.MapFrom(src => DateTime.UtcNow)) 
                .ForMember(dest => dest.FileName, opt => opt.Ignore()) 
                .ForMember(dest => dest.FileUrl, opt => opt.Ignore());
        }
    }
}
