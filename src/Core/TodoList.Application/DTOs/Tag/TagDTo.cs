using TodoList.Application.Ports.Validators;

namespace TodoList.Application.DTOs.Tag;

public class CreateTagDTo :  CommonPropertiesTagAndCategory 
{
    public string Color { get; set; } = "#FFFFFF";
}

public class UpdateTagDTo: CommonPropertiesTagAndCategory
{
    public int Id { get; set; }
    public string Color { get; set; } = "#FFFFFF";
 }

