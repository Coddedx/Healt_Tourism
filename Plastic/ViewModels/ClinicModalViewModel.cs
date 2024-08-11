using Plastic.Models;

namespace Plastic.ViewModels
{
    public class ClinicModalViewModel
    {
        public ClinicModalViewModel()  //constructor kullanarak vm null olduğuna veri tabanından çekilen veriyi vm e eşitlememizi sağlıyor yoksa Object reference not set to an instance of an object hatası alırız
        {
            this.Clinic = new Clinic();
            this.Doctor = new Doctor();
           //this.Franchise = new Franchise();
        }

        public Clinic? Clinic { get; set; }

        public Doctor? Doctor { get; set; }
        public IFormFile? Image { get; set; } //ıfromfile tipi veri gerektiği için 


       // public Franchise? Franchise { get; set; }

    }
}
