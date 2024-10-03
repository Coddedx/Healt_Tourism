using Plastic.Models;

namespace Plastic.ViewModels
{
    public class OperationDoctorViewModel
    {
        public OperationDoctor? OperationDoctor { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Operation> Operations { get; set; }
        public List<int> DoctorIds { get; set; } 
        public List<int> OperationIds { get; set; }
        public IFormFile? Image1 { get; set; }
        public IFormFile? Image2 { get; set; }
        public IFormFile? Image3 { get; set; }

        public string ClinicId { get; set; } 
        public string FranchiseId { get; set; } 

    }
}
