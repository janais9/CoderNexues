using System.ComponentModel.DataAnnotations;

namespace CoderNexues.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required] //  Student, Trainer, Assistant, Admin
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        //  (Navigation Properties)
        public ICollection<CampUser> CampUsers { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }
}
