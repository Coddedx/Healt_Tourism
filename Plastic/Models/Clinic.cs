using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Clinic : BaseEntity
	{
		[Key]
		public string Id { get; set; }

        public District District { get; set; } //virtual
        public int DistrictId { get; set; }  


        [Required, StringLength(64)]
		public string Name { get; set; }
        [Required]
        public string CertificationNumber { get; set; }
        [Required]
		public string Adress { get; set; }

		[Required, StringLength(64)]
		public string Email { get; set; }  // BUNU KALDIR IDENTİTY FRAMEWORK DAKİ EMAİL İ KULLAN

		[Required, StringLength(13)]
		public string Phone { get; set; }

		public virtual ICollection<Franchise?> Franchises { get; set; } = new List<Franchise>();	// Bir Klinik birden fazla Franchise'a sahip olabilir

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>(); // new List<Doctor>(); başlangıçta boş başlatılıyor model state de sorun olmuyor

        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
