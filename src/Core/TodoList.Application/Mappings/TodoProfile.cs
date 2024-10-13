using AutoMapper;
using TodoList.Application.DTOs.Todo;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TodoProfile: Profile
{
    public TodoProfile()
    {
        CreateMap<CreateTodoDTo, Todo>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Todo, TodoDTo>();
        CreateMap<Todo, CreateTodoDTo>();
    }
}