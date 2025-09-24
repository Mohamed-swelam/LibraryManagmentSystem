using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.CategoryDTOs
{
    public class recievedCategoryDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
    }
}
