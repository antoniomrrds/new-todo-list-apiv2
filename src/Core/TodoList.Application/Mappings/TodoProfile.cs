using AutoMapper;
using TodoList.Application.DTOs;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TodoProfile: Profile
{
    public TodoProfile()
    {
        CreateMap<TodoDto, Todo>();
    }
}