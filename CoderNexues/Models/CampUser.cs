using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class CampUser
    {
        [Key]
        public int CampUserID { get; set; }

        [ForeignKey("Camp")]
        public int CampID { get; set; }
        public Camp Camp { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public string RoleInCamp { get; set; } // Student, Trainer...
    }
}