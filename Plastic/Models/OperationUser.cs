using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Azure;

namespace Plastic.Models
{
	//migration sonrası
	public class OperationUser : BaseEntity   //bir kullanıcı birden fazla operasyon olabilir bir operasyonu birden fazla user olabilir
    {
		[Key]
        public int Id { get; set; }
        
		
		//[Required]
		public string AppUserId { get; set; } //int (identity framwork dan önce)
		//[ForeignKey("UserId")]
		public virtual AppUser? AppUser { get; set; }
		
		
		//[Required]
		public int OperationDoctorId { get; set; }
		//[ForeignKey("OperationDoctorId")]
		public virtual OperationDoctor? OperationDoctor { get; set; }

		
		public bool? Attended { get; set; }  //klinik/hastane ameliyatı yaptıysa bu true olcak ve kullanıcı ona göre yorum yapabilcek?????????  

		public decimal? Price { get; set; }  //kullanıcın bu operasyonu hangi ücretten olduğu burda ???????


		[StringLength(128)]
		public string? ImageUrl1 { get; set; }  // KULLANICIN BU OPERAYONLA İLGİLİ RESİMLERİ YAYINLANABİLİR
		[StringLength(128)]
		public string? ImageUrl2 { get; set; }
		[StringLength(128)]
		public string? ImageUrl3 { get; set; }

        //public virtual ICollection<OperationDoctor?> OperationDoctors { get; set; }

    }
}
