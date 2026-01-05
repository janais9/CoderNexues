using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class StudentPerformance
    {
        [Key]
        public int PerformanceID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public User Student { get; set; }

        [ForeignKey("Camp")]
        public int CampID { get; set; }
        public Camp Camp { get; set; }

        public decimal AvgScore { get; set; }
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}