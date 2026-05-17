using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.SavedLoad
{
    public class AddSavedLoadDTO
    {
        [Required] public int ProductId { get; set; }
    }
}
