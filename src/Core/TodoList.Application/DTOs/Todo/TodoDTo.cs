using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Todo;

public record CreateTodoDTo(
    int? IdTag,
    int? IdCategory,
    string Title,
    string Description,
    bool IsCompleted = false,
    int Status = DefaultValues.Active
);


//using System.ComponentModel.DataAnnotations;

// public class CreateTodoDTo
// {
//   [Required(ErrorMessage = "O ID da tag � obrigat�rio.")]
//   [Range(1, int.MaxValue, ErrorMessage = "O ID da tag deve ser um valor positivo.")]
//   public int IdTag { get; set; }
//
//   [Required(ErrorMessage = "O ID da categoria � obrigat�rio.")]
//   [Range(1, int.MaxValue, ErrorMessage = "O ID da categoria deve ser um valor positivo.")]
//   public int IdCategory { get; set; }
//
//   [Required(ErrorMessage = "O t�tulo � obrigat�rio.")]
//   [StringLength(100, ErrorMessage = "O t�tulo deve ter no m�ximo 100 caracteres.")]
//   public string Title { get; set; }
//
//   [Required(ErrorMessage = "A descri��o � obrigat�ria.")]
//   [StringLength(500, ErrorMessage = "A descri��o deve ter no m�ximo 500 caracteres.")]
//   public string Description { get; set; }
//
//   public bool IsCompleted { get; set; } = false;
//
//   [Range(0, 1, ErrorMessage = "O status deve ser 0 (inativo) ou 1 (ativo).")]
//   public int Status { get; set; } = DefaultValues.Active;
// }


public record TodoDTo(
    int Id,
    int? IdTag,
    int? IdCategory,
    string Title,
    string Description,
    bool IsCompleted,
    int Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm");
}