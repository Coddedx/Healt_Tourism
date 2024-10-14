using Plastic.Models;

namespace Plastic.ViewModels
{
    public class MessageViewModel
    {
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string SenderId { get; set; }
        public string MessageContent { get; set; }
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public MessageHistroyDetails MessageDetails { get; set; } = new MessageHistroyDetails();  //List<
    }

    public class MessageDto  //Message entity'sinden türetilmiş bir modeldir, ancak View'de ihtiyacınız olan alanları daha kolay erişilebilir hale getirmek için kullanılır. Bu şekilde, entity'nin tüm verilerini taşımak yerine, yalnızca gerekli bilgileri taşır
    {
        public string SenderId { get; set; }  //Mesajı gönderen kullanıcının ID'si (AppUser'dan alınır).
        public string SenderName { get; set; }
        //public string ReceiverName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class MessageHistroyDetails 
    {
        // public string Ids { get; set; }  //List<
        public List<User> UserDetails { get; set; }
        public List<Clinic> ClinicDetails { get; set; }
        public List<Franchise> FranchiseDetails { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
    }
}

