using Plastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.ViewModels
{
    public class FranchiseViewModel
    {
        //[Key]
        public int Id { get; set; }

        public List<Franchise> Franchises { get; set; }

        //[Required]
        //public int HospiatlId { get; set; }
        ////[ForeignKey("HospiatlId")]
        ////public virtual Hospital? Hospital { get; set; }


        ////[Required]
        //public int ClinicId { get; set; }
        ////[ForeignKey("ClinicId")]
        ////public virtual Clinic? Clinic { get; set; }


        //[Required]
        //public int DistrictId { get; set; }
        ////[ForeignKey("DistrictId")]
        ////public virtual District District { get; set; }


        //public string Title { get; set; }   
        //public string Description { get; set; }  
        //public string CertificationNumber { get; set; }

        //public string Adress { get; set; }

        //public string? ImageUrl { get; set; }

        //public string Email { get; set; }
        //public string Phone { get; set; }

        //public string? InstagramUrl { get; set; }

        ////public ICollection<Doctor> Doctors { get; set; }
        //public bool Status { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public int CreatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public int? UpdatedBy { get; set; }
        //public bool? Deleted { get; set; }


    }
}
