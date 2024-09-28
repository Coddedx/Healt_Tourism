using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Plastic.Models
{
    public class AppUser : IdentityUser
    {

        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        public string? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
        public string? FranchiseId { get; set; }
        public Franchise? Franchise { get; set; }


        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<OperationUser> OperationUsers { get; set; } = new List<OperationUser>();  //list olmalı -> koleksiyona ilk başta bir değer atanmamış olsa bile programın düzgün çalışmasını sağlar

    }
}
