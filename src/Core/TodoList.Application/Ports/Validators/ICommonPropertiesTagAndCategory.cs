using TodoList.Domain.Enums;

namespace TodoList.Application.Ports.Validators;

public abstract class CommonPropertiesTagAndCategory
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivationState Active { get;set; }
    public bool IsActive => Active == ActivationState.Active;

}
