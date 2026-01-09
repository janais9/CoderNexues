using System.ComponentModel.DataAnnotations;

namespace CoderNexues.Models
{
    public class Camp
    {
        [Key]
        public int CampID { get; set; }

        [Required(ErrorMessage = "اسم المعسكر مطلوب")]
        public string CampName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Active";

        public ICollection<CampUser>? CampUsers { get; set; }
        public ICollection<C_Task>? Tasks { get; set; }

        public ICollection<Announcement>? Announcements { get; set; }

    }
}