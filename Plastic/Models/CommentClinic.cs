using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
    public class CommentClinic :BaseEntity
    {
        [Required]
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }  //


        [Required]
        public string ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public virtual Clinic? Clinic { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int Star { get; set; } //int çünkü 1-5 arası star vericek ona göre animasyonu gösterilcek

    }
}
