using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile: Profile
{
    public TagProfile()
    {
        CreateMap<TagCreateDTo, Tag>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Tag, TagDTo>();
        CreateMap<Tag, TagCreateDTo>();
    }
}