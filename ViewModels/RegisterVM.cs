using System.ComponentModel.DataAnnotations;

namespace backend_app.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
