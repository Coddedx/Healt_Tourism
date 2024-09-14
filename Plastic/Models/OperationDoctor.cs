using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class OperationDoctor :BaseEntity // BİR OPERASYANU BİRDEN FAZLA DOKTOR YAPABİLİR BİR DOKTOR BİRDEN FAZLA OPERASYON YAPABİLİR
		//?????????????? Doktor Klinikten ayrılabilir ama o operasyonları başka doktor yapabilir o zaman operasyonları doktora bağlamamak mı gerekiyor??????????????????????  -->>>> franchise a bağlamalıyız?????????  --> ya da işlemi yapan doktoru da değiştirebilmeliyiz????????
	{
		[Key] 
        public int Id { get; set; }

        [Required]
		public int DoctorId { get; set; }
		[ForeignKey("DoctorId")]
		public virtual Doctor? Doctor { get; set; }

		[Required]
		public int OperationId { get; set; }
		[ForeignKey("OperationId")]
		public virtual Operation? Operation { get; set; }


		public decimal? DoctorPrice { get; set; }  //??????????? doktorun bu operasyonu hangi ücretten yaptığı burda????


		[StringLength(128)]
		public string? ImageUrl1 { get; set; }  // DOKTORUN BU OPERASYONLA İLGİLİ ÖRNEK RESİMLERİ YAYINLANABİLİR
		[StringLength(128)]
		public string? ImageUrl2 { get; set; }
		[StringLength(128)]
		public string? ImageUrl3 { get; set; }



        //public virtual ICollection<Franchise> Franchises { get; set; }
        //public virtual ICollection<Clinic> Clinics { get; set; }
    }
}
