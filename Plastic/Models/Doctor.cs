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


		[Required]
		public int FranchiseId { get; set; }  //????????????????????????????????????????
		[ForeignKey("FranchiseId")]
		public virtual Franchise? Franchise { get; set; }



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


		public ICollection<Operation> Operations { get; set; }


	}
}
