using Plastic.Models;

namespace Plastic.ViewModels
{
    public class ClinicModalViewModel
    {
        public ClinicModalViewModel()  //constructor kullanarak vm null olduğuna veri tabanından çekilen veriyi vm e eşitlememizi sağlıyor yoksa Object reference not set to an instance of an object hatası alırız
        {
            this.Clinic = new Clinic();
            this.Doctor = new Doctor();
            //this.Doctors = new List<Doctor>();
            this.OperationDoctor = new OperationDoctor();
           //this.Franchise = new Franchise();
        }

        public Clinic? Clinic { get; set; }

        public Doctor? Doctor { get; set; }
        //public List<Doctor?> Doctors { get; set; }
        public OperationDoctor? OperationDoctor { get; set; }

        public IFormFile? Image1 { get; set; } //ıfromfile tipi veri gerektiği için 
        public IFormFile? Image2 { get; set; } 
        public IFormFile? Image3 { get; set; } 


       // public Franchise? Franchise { get; set; }

    }
}
