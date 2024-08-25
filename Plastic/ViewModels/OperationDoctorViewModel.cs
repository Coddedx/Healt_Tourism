using Plastic.Models;

namespace Plastic.ViewModels
{
    public class OperationDoctorViewModel
    {
        public OperationDoctor? OperationDoctor { get; set; }
        public IFormFile? Image { get; set; }

        public int ClinicId { get; set; } 
        public int FranchiseId { get; set; } 

    }
}
