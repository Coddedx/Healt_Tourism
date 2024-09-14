using Plastic.Models;

namespace Plastic.ViewModels
{
    public class _PartialDoctorViewModel
    {
        public IEnumerable<Doctor> Doctors { get; set; }

        public Doctor EditDoctor { get; set; }

        public IFormFile? Image { get; set; } //ıfromfile tipi veri gerektiği için  
    }
}
