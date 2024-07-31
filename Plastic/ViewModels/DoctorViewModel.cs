using Plastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.ViewModels
{
    public class DoctorViewModel
    {
        //bu olduğunda formu açar açmaz direk uyarıları veriyor ve ıd created by falan tanımlamama rağmen 0 veriyor, bu olmadan direk null diyo doctor a 
        //public DoctorViewModel()  //constructor kullanarak vm null olduğuna veri tabanından çekilen veriyi vm e eşitlememizi sağlıyor yoksa Object reference not set to an instance of an object hatası alırız
        //{
        //    this.Doctor = new Doctor();
        //}

        public Doctor? Doctor { get; set; } //clinic formunda gelen verileri model state doğru değilse geri döndürürken böyle yapamadığım için tek tek yazdım
        //public int FranchiseId { get; set; }  
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Gender { get; set; }
        //public string Phone { get; set; }
        //public string Title { get; set; }
        //public string Country { get; set; }  
        //public string CertificationNumber { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        //public bool Status { get; set; }


        public int ClinicId { get; set; } //bunu aktarmam gerekiyor tekrardan clinic controllerına 

    }
}
