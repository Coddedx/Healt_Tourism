using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Franchise :BaseEntity
	{

		[Key]
		public int Id { get; set; }

		[Required]
		public int? HospiatlId { get; set; }  // ya hastanenin ya da kliniğin franchise olabilir!!!!!!!!!!!!!!!!!!!!!!!
		[ForeignKey("HospiatlId")]
		public virtual Hospital? Hospital { get; set; }


		[Required]
		public int? ClinicId { get; set; }
		[ForeignKey("ClinicId")]
		public virtual Clinic? Clinic { get; set; }


		[Required]
		public int DistrictId { get; set; }
		[ForeignKey("DistrictId")]
		public virtual District District { get; set; }


		[Required, StringLength(64)]
		public string Title { get; set; }   //ayrı tablo?????????????????????????????
        [Required]
		public string Description { get; set; }  //ayrı tablo?????????????????????????????
		[StringLength(128)]

        [Required]
        public string CertificationNumber { get; set; }
        
		[Required, StringLength(128)]
        public string Address { get; set; }

		[StringLength(128)]
		public string? ImageUrl { get; set; }

		[Required, StringLength(128)]
		public string Email { get; set; }
		[Required, StringLength(13)]
		public string Phone { get; set; }

        public string? InstagramUrl { get; set; }

        public ICollection<Doctor> Doctors { get; set; }


	}
}
