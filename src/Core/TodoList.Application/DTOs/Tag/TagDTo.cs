using TodoList.Application.Ports.Validators;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Tag;

public class CreateTagDTo :  CommonPropertiesTagAndCategory
{
}

public class UpdateTagDTo: CommonPropertiesTagAndCategory
{
    public int Id { get; set; }
 }


public class TagFilterDTo
{
    public int Page { get; set; }
    public int IdUser { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PageSize { get; set; }
    public ActivationState Active { get; set; }
    public bool IsActive => Active == ActivationState.Active;
}
