using Microsoft.AspNetCore.Http;

namespace TodoList.Application.DTOs.User;

public record UserChangeImageRequestDTo(IFormFile Image);
public record UserUpdateImageDTo(int Id, string  ImageUrl ,  Guid FileId);
