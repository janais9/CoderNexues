using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PostedAt { get; set; } = DateTime.Now;

        [ForeignKey("Camp")]
        public int CampID { get; set; }
        public Camp Camp { get; set; }
    }
}