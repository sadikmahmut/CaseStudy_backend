using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace backend_app.Model
{   
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }
    }
}
