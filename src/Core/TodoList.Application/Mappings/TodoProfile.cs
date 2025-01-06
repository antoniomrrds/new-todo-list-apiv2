using AutoMapper;
using TodoList.Application.DTOs.Todo;

using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TodoProfile: Profile
{
    public TodoProfile()
    {
        
        CreateMap<UpdateTodoDTo, TodoWithTagAndCategoryIdsDto>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        CreateMap<Todo, CreateTodoDTo>();
        CreateMap<TodoWithTagAndCategoryIdsDto, UpdateTodoDTo>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}