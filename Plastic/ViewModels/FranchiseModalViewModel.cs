using Plastic.Models;

namespace Plastic.ViewModels
{
    public class FranchiseModalViewModel
    {
        public FranchiseModalViewModel()
        {
            this.Franchise = new Franchise();
            this.Doctor = new Doctor();
            this.OperationDoctor = new OperationDoctor();
        }
        public Franchise? Franchise { get; set; }
        public Doctor? Doctor { get; set; }
        public OperationDoctor? OperationDoctor { get; set; }
        public IFormFile? Image { get; set; } //ıfromfile tipi veri gerektiği için 

    }
}
