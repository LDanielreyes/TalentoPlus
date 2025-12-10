using System.ComponentModel.DataAnnotations;

namespace TalentoPlusWeb.Models;

public class WorkerImportViewModel
{
    [Required(ErrorMessage = "Por favor seleccione un archivo.")]
    [Display(Name = "Archivo Excel")]
    public IFormFile File { get; set; }
}
