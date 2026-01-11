using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class StudentCertificate
    {
        [Key]
        public int CertID { get; set; }

        public int ReqID { get; set; } 
        [ForeignKey("ReqID")]
        public CertificateRequirement Requirement { get; set; }

        public int StudentID { get; set; } 
        [ForeignKey("StudentID")]
        public User Student { get; set; }

        public bool HasTakenExam { get; set; } 
        public DateTime? ExamDate { get; set; } 
        public int? Score { get; set; } 
        public bool IsPassed { get; set; } 
    }
}