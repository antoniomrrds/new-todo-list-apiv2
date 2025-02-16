using AutoMapper;
using TodoList.Application.DTOs.Auth;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<SignUpDTo, User>();
    }
}
