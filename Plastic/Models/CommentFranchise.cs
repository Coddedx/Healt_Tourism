using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
    public class CommentFranchise :BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }


        [Required]
        public int FranchiseId { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise? Franchise { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int Star { get; set; } 

    }
}
