using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Clinic : BaseEntity
	{
		[Key]
		public int Id { get; set; }

        public District District { get; set; } //virtual
        public int DistrictId { get; set; }  //?????????????????????????


        [Required, StringLength(64)]
		public string Name { get; set; }
        [Required]
        public string CertificationNumber { get; set; }
        [Required]
		public string Adress { get; set; }

		[Required, StringLength(64)]
		public string Email { get; set; }

		[Required, StringLength(13)]
		public string Phone { get; set; }

		public virtual ICollection<Franchise?> Franchises { get; set; }		// Bir Klinik birden fazla Franchise'a sahip olabilir

        public virtual ICollection<Doctor> Doctors { get; set; }

    }
}
