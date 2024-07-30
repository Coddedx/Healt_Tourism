using Plastic.Models;

namespace Plastic.ViewModels
{
    public class DoctorViewModel
    {
        public Doctor? Doctor { get; set; }
        public int ClinicId { get; set; } //bunu aktarmam gerekiyor tekrardan clinic controllerına 

    }
}
