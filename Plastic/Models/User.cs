using System.ComponentModel.DataAnnotations;

namespace Plastic.Models
{
	public class User : BaseEntity
	{
		[Key]
		public string Id { get; set; }  //appUser ile ıd si aynı olacak kendi kendine artamaz
		
		[Required,
		StringLength(64, ErrorMessage = "Max length should be 64 character.")
		,MinLength(3, ErrorMessage = "Min length should be 3 character.")]
		public string FirstName { get; set; }

		[Required,
		StringLength(64, ErrorMessage = "Max length should be 64 character.")
		, MinLength(3, ErrorMessage = "Min length should be 3 character.")]
		public string LastName { get; set; }

		[Required]
        public string Gender { get; set; }        
		
		public string? IdentityNumber { get; set; }

        public string? Country { get; set; }


		//public string? Phone { get; set; }  //identity framework bu özelliği sağlıyor

  //      [Required]
		//public string Email { get; set; }

		//[Required,
		//StringLength(32, ErrorMessage = "Max length should be 32 character.")
		//,MinLength(8, ErrorMessage = "Min length should be 8 character.")]
		//public string Password { get; set; }


        public string? Image { get; set; }

        public bool lawReasons { get; set; }

        public virtual ICollection<OperationUser?> OperationUsers { get; set; }
        public virtual ICollection<AppUser> Users { get; set; }

    }
}
