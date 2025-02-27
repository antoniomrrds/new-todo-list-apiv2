using AutoMapper;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.DTOs.User;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<SignUpDTo, User>();
        CreateMap<User, UserResponseWithoutPasswordDTo>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}
