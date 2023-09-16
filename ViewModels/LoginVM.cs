using System.ComponentModel.DataAnnotations;

namespace backend_app.ViewModels
{
    public class LoginVM
    {
        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
