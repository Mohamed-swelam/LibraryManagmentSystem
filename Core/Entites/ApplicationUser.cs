using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entites
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(20),MinLength(7)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(20), MinLength(7)]
        public required string LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime JoinedDate { get; set; }

        public ICollection<Borrowings>? Borrowings { get; set; }

    }
}
