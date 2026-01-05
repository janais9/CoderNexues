using System.ComponentModel.DataAnnotations;

namespace CoderNexues.Models
{
    public class FeedbackCategory
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } // Strength, Improvement, Attendance
    }
}