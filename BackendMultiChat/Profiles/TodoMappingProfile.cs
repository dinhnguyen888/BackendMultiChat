using AutoMapper;
using BackendMultiChat.Dtos;
using BackendMultiChat.Models;

namespace BackendMultiChat.Profiles
{
    public class TodoMappingProfile : Profile
    {
        public TodoMappingProfile()
        {
            // TodoItem
            CreateMap<TodoItem, TodoItemGetDto>();
            CreateMap<TodoItemPostDto, TodoItem>();
            CreateMap<TodoItemUpdateDto, TodoItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // TodoList
            CreateMap<TodoList, TodoListGetDto>()
                .ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => src.PriorityLevel.ToString()))
                .ForMember(dest => dest.TodoItems, opt => opt.MapFrom(src => src.TodoItems));
            CreateMap<TodoListPostDto, TodoList>()
                .ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => Enum.Parse<TodoList.Priority>(src.PriorityLevel)));
            CreateMap<TodoListUpdateDto, TodoList>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
