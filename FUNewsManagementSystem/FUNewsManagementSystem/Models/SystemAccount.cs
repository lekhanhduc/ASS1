using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FUNewsManagementSystem.Common;

namespace FUNewsManagementSystem.Models
{
    public class SystemAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }

        public UserStatus userStatus { get; set; } = UserStatus.ACTIVE;

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; } // 1: Staff, 2: Lecturer, Admin
    }

}
