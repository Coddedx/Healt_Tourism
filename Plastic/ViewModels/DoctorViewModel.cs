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

        public IFormFile? Image { get; set; }

        public int ClinicId { get; set; } //bunu aktarmam gerekiyor tekrardan clinic controllerına 

    }
}
