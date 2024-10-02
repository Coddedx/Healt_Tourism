using Plastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plastic.ViewModels
{
    public class FranchiseViewModel
    {
        //[Key]
        public int Id { get; set; }

        //public List<Franchise> Franchises { get; set; }
        public Franchise Franchise { get; set; }

    }
}
