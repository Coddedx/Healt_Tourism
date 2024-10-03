using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
    public class CommentFranchise :BaseEntity
    {
        [Required]
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }  //User


        [Required]
        public string FranchiseId { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise? Franchise { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int Star { get; set; } 

    }
}
