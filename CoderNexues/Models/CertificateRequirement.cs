using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoderNexues.Models
{
    public class CertificateRequirement
    {
        [Key]
        public int ReqID { get; set; }

        public int CampID { get; set; }
        [ForeignKey("CampID")]
        public Camp Camp { get; set; }

        [Required]
        public string CertificateName { get; set; } 

        [Required]
        public DateTime Deadline { get; set; } 

        public int PassingScore { get; set; } 
    }
}