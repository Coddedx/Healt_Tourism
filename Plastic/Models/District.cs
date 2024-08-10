using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class District
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		public int CityId { get; set; }
		[ForeignKey("CityId")]
		public virtual City City { get; set; }

		
		[Required, StringLength(64)]
		public string Name { get; set; }

		public ICollection<Franchise> Franchises { get; set; }
		public ICollection<Clinic> Clinics { get; set; }

	}
}
