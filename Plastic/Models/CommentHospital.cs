using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
    public class CommentHospital :BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }


        [Required]
        public int HospitalId { get; set; }
        [ForeignKey("HospitalId")]
        public virtual Hospital? Hospital { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int Star { get; set; }

    }
}
