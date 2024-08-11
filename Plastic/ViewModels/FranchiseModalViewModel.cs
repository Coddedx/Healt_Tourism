using Plastic.Models;

namespace Plastic.ViewModels
{
    public class FranchiseModalViewModel
    {
        public FranchiseModalViewModel()
        {
            this.Franchise = new Franchise();
            this.Doctor = new Doctor();
        }
        public Franchise? Franchise { get; set; }
        public Doctor? Doctor { get; set; }
        public IFormFile? Image { get; set; } //ıfromfile tipi veri gerektiği için 

    }
}
