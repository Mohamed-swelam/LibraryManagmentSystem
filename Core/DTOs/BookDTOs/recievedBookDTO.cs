using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.BookDTOs
{
    public class recievedBookDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required string Author { get; set; }
        public int TotalCopies { get; set; }
        public int AvaliableCopies { get; set; }
        public int? CategoryId { get; set; }
    }
}
