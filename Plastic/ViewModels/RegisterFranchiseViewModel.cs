using Plastic.Models;
using System.ComponentModel.DataAnnotations;

namespace Plastic.ViewModels
{
    public class RegisterFranchiseViewModel
    {
        public Franchise Franchise { get; set; }
       
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }


        [Required, DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "District is required")]
        public int DistrictId { get; set; }  // DistrictId ekleniyor
        public int ClinicId { get; set; }  // DistrictId ekleniyor


    }
}
