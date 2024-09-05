using Plastic.Models;

namespace Plastic.ViewModels
{
    public class _PartialDoctorViewModel
    {
        public IEnumerable<Doctor> Doctors { get; set; }

        public Doctor EditDoctor { get; set; }

        public IFormFile? Image1 { get; set; } //ıfromfile tipi veri gerektiği için 
        public IFormFile? Image2 { get; set; }
        public IFormFile? Image3 { get; set; }
    }
}
