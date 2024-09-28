using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
    public class CommentHospital :BaseEntity
    {
        [Required]
        public string AppUserId { get; set; } //string
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }  //User 


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
