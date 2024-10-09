using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile: TodoProfile
{
    public TagProfile()
    {
        CreateMap<TagDTo, Tag>();
    }
}