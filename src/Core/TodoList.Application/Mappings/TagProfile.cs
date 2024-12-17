using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile: Profile
{
    public TagProfile()
    {
        CreateMap<CreateTagDTo,Tag >();
        CreateMap<UpdateTagDTo,Tag>();
    }
}