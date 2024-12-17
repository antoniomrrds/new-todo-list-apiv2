using TodoList.Application.DTOs.Tag;
using TodoList.Application.Validators.Shared;

namespace TodoList.Application.Validators.Tag;

public class CreateTagDToValidator : CommonTagAndCategoryValidator<CreateTagDTo>;

public class UpdateTagDToValidator : CommonTagAndCategoryValidator<UpdateTagDTo>;