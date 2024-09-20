using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plastic.Models
{
	public class Operation
	{
		[Key]
        public int Id { get; set; }

		[Required]
		public int CategoryId { get; set; }  
		[ForeignKey("CategoryId")]
		public virtual Category? Category { get; set; }


		[Required]
        public string Title { get; set; }

		[Required]
		public string Description { get; set; }

        public decimal? Price { get; set; }  


        [StringLength(128)]
		public string? ImageUrl1 { get; set; }  // KATEGORİDE YAYINLANMASI İÇİN ÖRNEK RESİMLER OLABİLİR
		[StringLength(128)]
		public string? ImageUrl2 { get; set; }
		[StringLength(128)]
		public string? ImageUrl3 { get; set; }

        public virtual ICollection<OperationDoctor?> OperationDoctors { get; set; }

    }
}
