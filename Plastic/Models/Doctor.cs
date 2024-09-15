using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plastic.Models
{
	public class Doctor : BaseEntity
	{
		[Key]
		public int Id { get; set; }


		//[Required]
		//public int HospitalId { get; set; }
		//[ForeignKey("HospitalId")]
		//public virtual Hospital? Hospital { get; set; }  // Ya buna ya franchise e bağlı olcak ?????????????????!!!!!!!!!!!!!!!!


		//[Required]
		//[ForeignKey("FranchiseId")]
		//public  Franchise Franchise { get; set; } //virtual
  //      public int FranchiseId { get; set; }  //????????????????????????????????????????



        // Nullable foreign key for Franchise   BİR DOKTOR YA FRANCHİSE A YA CLİNİC E BAĞLI OLABİLİR 
        public int? FranchiseId { get; set; }
        public virtual Franchise Franchise { get; set; }



        // Nullable foreign key for Clinic
        public int? ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        //FranchiseId ve ClinicId nullable olarak tanımlandığı için bir doktor sadece bir franchise veya bir clinic ile ilişkilendirilebilir. Her ikisi de null olabilir, ancak her ikisi de aynı anda dolu olmamalıdır.
        //Bu durumda, Franchise ya da Clinic verisinin yalnızca biri atanmalıdır. ama bir doktor 2 yerde çalışabilir ????? o yüzden gerek yok ???????????
        //public void Validate()
        //{
        //    if (FranchiseId.HasValue && ClinicId.HasValue)
        //    {
        //        throw new InvalidOperationException("A doctor cannot be associated with both a franchise and a clinic at the same time.");
        //    }
        //}


        [Required,
		StringLength(64, ErrorMessage = "Max length should be 64 character.")
		, MinLength(3, ErrorMessage = "Min length should be 3 character.")]
		public string FirstName { get; set; }

		[Required,
		StringLength(64, ErrorMessage = "Max length should be 64 character.")
		, MinLength(3, ErrorMessage = "Min length should be 3 character.")]
		public string LastName { get; set; }

		[Required]
		public string Gender { get; set; }

		[Required]
		public string Phone { get; set; }

        public string Title { get; set; }

		public string Country { get; set; }  //bunu aslında country tablosundan çekebiliriz?????????????????????????

		[Required]
        public string CertificationNumber { get; set; }  

        [Required]
		public string Email { get; set; }

		[Required,
		StringLength(32, ErrorMessage = "Max length should be 32 character.")
		, MinLength(8, ErrorMessage = "Min length should be 8 character.")]
		public string Password { get; set; }

        //[StringLength(128)]
        public string? ImageUrl { get; set; } //IFormFile doğrudan veritabanına kaydedilemez çünkü bu tip yalnızca HTTP istekleriyle gönderilen dosyaları temsil eder. (cloudinary için ıfromfile gerkliydi) 


		//public ICollection<OperationDoctor?> OperationDoctors { get; set; }


	}
}
