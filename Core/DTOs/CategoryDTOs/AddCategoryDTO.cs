using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.CategoryDTOs
{
    public class AddCategoryDTO
    {
        [Required]
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}
