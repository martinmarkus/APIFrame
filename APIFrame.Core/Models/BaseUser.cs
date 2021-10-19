using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFrame.Core.Models
{
    [Table("users")]
    public class BaseUser : BaseEntity
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public int Balance { get; set; } = 0;

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required]
        public string AuthIP { get; set; } = "N/A";

        [Required]
        public DateTime AuthDate { get; set; } = DateTime.Now;

        [Required]
        public bool IsBanned { get; set; } = false;
    }
}
