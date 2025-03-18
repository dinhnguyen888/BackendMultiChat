using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
          
            CreateMap<Account, AccountGetDto>();
            CreateMap<Account, ProfileGetDto>();

            CreateMap<AccountPostDto, Account>();

            //Map from AccountUpdateDto to Account
            CreateMap<AccountUpdateDto, Account>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
      
        }
    }
}
