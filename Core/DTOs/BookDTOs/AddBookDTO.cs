using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.BookDTOs
{
    public class AddBookDTO
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required string Author { get; set; }
        [Required]
        public int TotalCopies { get; set; }
        public int? CategoryId { get; set; }
    }
}
