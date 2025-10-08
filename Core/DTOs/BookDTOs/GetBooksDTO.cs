using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.BookDTOs
{
    public class GetBooksDTO
    {
        [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        public string? testFilter { get; set; }

    }
}
