using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class RegisterDTO
    {
        [EmailAddress(ErrorMessage ="Invalid Email Format..")]
        [Required(ErrorMessage = "Email is Required..")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is Required..")]
        [DataType(DataType.Password)]
        [StringLength(35,MinimumLength =6,ErrorMessage = "The Password must be between 6 and 25")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FirstName is Required..")]
        [MaxLength(25,ErrorMessage ="FirstName must be less Than 25..")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is Required..")]
        [MaxLength(25, ErrorMessage = "LastName must be less Than 25..")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required..")]
        [Phone(ErrorMessage = "Invalid PhoneNumber format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is Required..")]
        public string Address { get; set; }

        [Required(ErrorMessage = "DateofBirth is Required..")]
        public DateTime? DateofBirth { get; set; }
    }
}
