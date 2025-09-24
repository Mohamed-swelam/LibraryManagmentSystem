namespace Core.DTOs.BookDTOs
{
    public class BookResponseDTO
    {
        public int BookId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Author { get; set; }
        public int TotalCopies { get; set; }
        public int AvaliableCopies { get; set; }
        public int? CategoryId { get; set; }
    }
}
