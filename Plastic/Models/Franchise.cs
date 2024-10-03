using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Franchise :BaseEntity
	{

		[Key]
		public string Id { get; set; }

		//[Required]
		//[ForeignKey("HospitalId")]
		//public virtual Hospital? Hospital { get; set; }
  //      public int HospitalId { get; set; }  // ya hastanenin ya da kliniğin franchise olabilir!!!!!!!!!!!!!!!!!!!!!!!


        //Clinic birden fazla Franchise ile ilişkilendirilebilir. Her Franchise bir Clinic ile ilişkilidir ve bu ilişki ClinicId üzerinden yönetilir.
        //[Required]
        //[ForeignKey("ClinicId")]
        public Clinic Clinic { get; set; }  //virtual
        public string ClinicId { get; set; }



        //[Required]
		//[ForeignKey("DistrictId")]
		public District District { get; set; } //virtual
        public int DistrictId { get; set; }



        [Required, StringLength(64)]
		public string Title { get; set; }   //ayrı tablo?????????????????????????????
        [Required]
		public string Description { get; set; }  //ayrı tablo?????????????????????????????
		[StringLength(128)]
       
		[Required]
        public string Adress { get; set; }

        [Required]
        public string CertificationNumber { get; set; }
        
		[StringLength(128)]
		public string? ImageUrl { get; set; }

		[Required, StringLength(128)]
		public string Email { get; set; }
		[Required, StringLength(13)]
		public string Phone { get; set; }

        public string? InstagramUrl { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();

    }
}
