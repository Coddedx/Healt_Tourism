using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Azure;

namespace Plastic.Models
{
	//migration sonrası
	public class OperationUser : BaseEntity   //bir kullanıcı birden fazla operasyon olabilir bir operasyonu birden fazla user olabilir
    {
        [Required]
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public virtual User? User { get; set; }
		
		[Required]
		public int OperationId { get; set; }
		[ForeignKey("OperationId")]
		public virtual Operation? Operation { get; set; }

		
		public bool? Attended { get; set; }  //klinik/hastane ameliyatı yaptıysa bu true olcak ve kullanıcı ona göre yorum yapabilcek?????????  

		public decimal? Price { get; set; }  //kullanıcın bu operasyonu hangi ücretten olduğu burda ???????


		[StringLength(128)]
		public string? ImageUrl1 { get; set; }  // KULLANICIN BU OPERAYONLA İLGİLİ RESİMLERİ YAYINLANABİLİR
		[StringLength(128)]
		public string? ImageUrl2 { get; set; }
		[StringLength(128)]
		public string? ImageUrl3 { get; set; }

	}
}
