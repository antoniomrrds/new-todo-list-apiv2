using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.DTOs.Todo;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile: Profile
{
    public TagProfile()
    {
        CreateMap<UpdateTodoDTo, TodoWithTagAndCategoryIdsDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Tag, TagDTo>();
        CreateMap<Tag, CreateTagDTo>();
    }
}