using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class C_Task
    {
        [Key]
        public int TaskID { get; set; }

        [ForeignKey("Camp")]
        public int CampID { get; set; }
        public Camp Camp { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }

        public ICollection<Submission> Submissions { get; set; }
    }
}