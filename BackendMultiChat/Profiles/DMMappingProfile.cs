using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class DMMappingProfile : Profile
    {
        public DMMappingProfile()
        {

            CreateMap<DirectMessage, DMGetDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.FullName));


            CreateMap<DMPostDto, DirectMessage>()
                .ForMember(dest => dest.SentDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.FileName, opt => opt.Ignore())
                .ForMember(dest => dest.FileUrl, opt => opt.Ignore());
        }
    }
}
