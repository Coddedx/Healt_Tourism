using Plastic.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Plastic.ViewModels
{
    public class ClinicViewModel
    {
        [Key]
        public int Id { get; set; }

        public List<Clinic> Clinics { get; set; }
        public List<Hospital> Hospitals { get; set; }


    }
}
