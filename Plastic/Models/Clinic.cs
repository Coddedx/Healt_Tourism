using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Clinic : BaseEntity
	{
		[Key]
		public int Id { get; set; }

		[Required, StringLength(64)]
		public string Name { get; set; }
        [Required]
        public string CertificationNumber { get; set; }
        [Required, StringLength(128)]
		public string Adress { get; set; }

		[Required, StringLength(64)]
		public string Email { get; set; }

		[Required, StringLength(13)]
		public string Phone { get; set; }

	}
}
