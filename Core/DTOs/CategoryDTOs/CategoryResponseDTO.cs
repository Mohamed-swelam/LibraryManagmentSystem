using Core.DTOs.BookDTOs;
using Core.Entites;

namespace Core.DTOs.CategoryDTOs
{
    public class CategoryResponseDTO
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public List<BookResponseDTO>? Books { get; set; }
    }
}
