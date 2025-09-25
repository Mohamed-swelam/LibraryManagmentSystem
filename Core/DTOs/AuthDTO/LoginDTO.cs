using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
