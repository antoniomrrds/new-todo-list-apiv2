using AutoMapper;
using TodoList.Application.DTOs.Category;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CreateCategoryDTo, Category>();
    }

}