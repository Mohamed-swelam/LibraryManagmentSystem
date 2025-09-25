using Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entites
{
    public class Borrowings
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required int BookId { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowingStatus Status { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}