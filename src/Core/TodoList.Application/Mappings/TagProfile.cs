using TodoList.Application.DTOs;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile: TodoProfile
{
    public TagProfile()
    {
        CreateMap<TagDTo, Tag>();
    }
}