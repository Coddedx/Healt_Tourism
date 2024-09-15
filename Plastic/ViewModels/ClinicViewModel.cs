using Plastic.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Plastic.ViewModels
{
    public class ClinicViewModel
    {
        //[Key]
        //public int Id { get; set; }

        //public ClinicViewModel()  //constructor kullanarak vm null olduğuna veri tabanından çekilen veriyi vm e eşitlememizi sağlıyor yoksa Object reference not set to an instance of an object hatası alırız
        //{
        //    //this.Clinics = new List<Clinic>();
        //    //this.Franchises = new List<Franchise>();
        //    this.Clinic = new Clinic();
        //    this.Franchise = new Franchise();
        //}

        public List<Clinic> Clinics { get; set; } //= new List<Clinic>(); //IEnumerable<  
        public Clinic? Clinic { get; set; }
        
        public Franchise? Franchise { get; set; }
        public List<Franchise> Franchises { get; set; } //= new List<Franchise>();  //List<   = new List<Franchise>()
        
        public Pager Pager { get; set; }
    }
}
