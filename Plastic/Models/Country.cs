using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Country
	{
		[Key]
		public int Id { get; set; }

		[Required, StringLength(64)]
		public string Name { get; set; }

		[Required, StringLength(3)]
		public string Code { get; set; }

	}
}
