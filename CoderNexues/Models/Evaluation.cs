using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class Evaluation
    {
        [Key]
        public int EvaluationID { get; set; }

        [ForeignKey("Submission")]
        public int SubmissionID { get; set; }
        public Submission Submission { get; set; }

        [ForeignKey("Evaluator")]
        public int EvaluatorID { get; set; }
        public User Evaluator { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public FeedbackCategory Category { get; set; }

        public int Score { get; set; }
        public string Feedback { get; set; }
        public DateTime EvaluatedAt { get; set; } = DateTime.Now;
    }
}