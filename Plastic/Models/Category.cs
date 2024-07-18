using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class Category : BaseEntity  //migration sonrası
	{
		public Category()
		{
			this.Operations = new HashSet<Operation>(); //hashset default collection oluşturuyor aldığı değeri tekrar tekrar almıyor
		}

		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }
		[Required]
		public string Description { get; set; }
		public string? ImageUrl { get; set; }

		public virtual ICollection<Operation> Operations { get; set; }

	}
}
