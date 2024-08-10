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
        //    this.Clinics = new List<Clinic>();
        //    this.Franchises = new List<Franchise>();
        //}

        public List<Clinic> Clinics { get; set; } //= new List<Clinic>(); //IEnumerable<  
        public List<Franchise> Franchises { get; set; } //= new List<Franchise>();  //List<   = new List<Franchise>()

        //public List<Hospital> Hospitals { get; set; }


    }
}
