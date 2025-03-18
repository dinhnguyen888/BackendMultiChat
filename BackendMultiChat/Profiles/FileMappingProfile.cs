using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class FileMappingProfile : Profile
    {
        public FileMappingProfile()
        {
            CreateMap<FileStorage, FileGetDto>();
        }
    }
}
