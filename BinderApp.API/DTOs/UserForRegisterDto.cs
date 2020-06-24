using System.ComponentModel.DataAnnotations;

namespace BinderApp.API.DTOs
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Your password must be between 4 and 8 characters!")]
        public string Passowrd { get; set; }
    }
}