using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionID { get; set; }

        [ForeignKey("Task")]
        public int TaskID { get; set; }
        public C_Task Task { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public User Student { get; set; }

        public string SubmissionLink { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Submitted"; // Submitted, Late

        public Evaluation Evaluation { get; set; }
    }
}