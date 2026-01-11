using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class CampSchedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [ForeignKey("Camp")]
        public int CampID { get; set; }
        public Camp Camp { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } = "Lecture";
    }
}