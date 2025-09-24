using System.ComponentModel.DataAnnotations;

namespace Core.Entites
{
    public class Category
    {
        public int CategoryId { get; set; }
        [MaxLength(30)]
        public required string Name { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
