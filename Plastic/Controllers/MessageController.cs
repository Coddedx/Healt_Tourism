using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Plastic.Hubs;
using Plastic.Models;
using Plastic.Services;
using Plastic.ViewModels;
using System.Security.Claims;

namespace Plastic.Controllers
{
    public class MessageController : Controller
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly PlasticDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<AppUser> _userManager;


        public MessageController(PlasticDbContext context, RabbitMqService rabbitMqService, IHubContext<ChatHub> hubContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
            _hubContext = hubContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string id, string SenderId) //id receiver id,string SenderId,string ReceiverId 
        {
            // CLİNİC VE FRANCHİSE İÇİN SEDNERID GELME DURUMU CLİNİC VEYA FRANCHİSE OLARAK GİRİŞ YAPILDIĞINDA MESAJLARIM GİBİ OLAN KUTUCUĞA BASILDIĞINDA GELİCEK ŞU AN SADECE USER MESAJ ATIYORRRRR!!!!!!
            //var userId = _userManager.GetUserId(User);

            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //claim, genellikle kullanıcının veritabanındaki Id alanına denk gelir GİRİŞ YAPAN HANGİ USER BULUNUYOR DİREK İD YE GEREK OLMADAN 
           
            //var senderUser = _context.Users.Where(b => b.Id == id);
            // var senderUserId = senderUser.Select(c => c.Id);
            var senderClinic = _context.Clinics.Where(b => b.Id == id); //  SenderId FirstOrDefaultAsync(c => c.Id == id)
            var senderClinicId = senderClinic.Select(c => c.Id);
            var senderFranchise = _context.Franchises.Where(b => b.Id == id);  // SenderId
            var senderFranchiseId = senderFranchise.Select(b => b.Id);

            string senderId = "";
            string senderName = "Bilinmeyen???";
            if (senderUserId != null)
            {
                //senderId = $"{senderUserId}";
                senderId = senderUserId;
                var senderUserr = _context.Users.FirstOrDefault(u => u.Id == senderId);
                senderName = $"{senderUserr.FirstName} {senderUserr.LastName}";
            }
            else if (senderClinicId != null)
            {
                senderId = $"{senderClinicId}";
                var senderClinicc = _context.Clinics.Include(a => a.District).FirstOrDefault(a => a.Id == senderId);
                senderName = senderClinicc.Name + "-" + senderClinicc.District.City.Name + "(" + senderClinicc.District.Name + ")";
            }
            else if (senderFranchiseId != null)
            {
                senderId = $"{senderFranchiseId}";
                var senderFranchisee = _context.Franchises.Include(a => a.District).FirstOrDefault(a => a.Id == senderId);
                senderName = senderFranchisee.Title + "-" + senderFranchisee.District.City.Name + "(" + senderFranchisee.District.Name + ")";
            }

            var messages = _context.Messages
                .Include(m => m.Sender)
                .Where(m => (m.SenderId == senderId && m.ReceiverId == id) || (m.SenderId == id && m.ReceiverId == senderId))
                .OrderBy(m => m.SenAt)
                .ToList();

            var messagesToUpdate = _context.Messages
                .Where(m => ((m.SenderId == senderId && m.ReceiverId == id) ||
                (m.SenderId == id && m.ReceiverId == senderId)) &&
                m.Read == false) // Sadece okunmamış mesajlar
                .ToList();
            foreach (var message in messagesToUpdate)
            {
                message.Read = true;
            }
            await _context.SaveChangesAsync();

            var messageVM = new MessageViewModel();

            var receiverUser = await _context.Users.FindAsync(id);
            var receiverClinic = await _context.Clinics.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefaultAsync(a => a.Id == id);//;FindAsync(id)
            var receiverFranchise = await _context.Franchises.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefaultAsync(a => a.Id == id);  //FindAsync(id)


            //var receiver = receiverUser;
            if (messages != null)
            {
                messageVM.ReceiverId = id;
                if (receiverUser != null)
                {
                    messageVM.ReceiverName = receiverUser?.FirstName + "" + receiverUser?.LastName;
                }
                else if (receiverClinic != null)
                {
                    messageVM.ReceiverName = receiverClinic?.Name + "" + receiverClinic?.District.City.Name + "-" + receiverClinic?.District.Name;
                }
                else if (receiverFranchise != null)
                {
                    messageVM.ReceiverName = receiverFranchise?.Title + "" + receiverFranchise?.District.City.Name + "-" + receiverFranchise?.District.Name;
                }
                messageVM.SenderId = senderId;
                messageVM.Messages = messages.Select(m => new MessageDto
                {
                    SenderId = m.SenderId,
                    SenderName = senderName,//m.Sender != null ? m.Sender.User.FirstName + " " + m.Sender.User.LastName : "Bilinmeyen???", //sender ın null olup olmadığı kotnrol edilip değilse isim soy isim birleştirliyor
                    Content = m.Content,
                    SentAt = m.SenAt
                }).ToList();
            }



            // sender receiver clinic/franchise ya da user olduğu kontrolü yapılmalı !!!!!!!!!!!!!!!!!!!!!!????????????????????????

            return View(messageVM);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageViewModel messageVM)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = messageVM.ReceiverId,
                Content = messageVM.MessageContent,
                SenAt = DateTime.UtcNow
            };

            // Mesajı veritabanına ekle
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // RabbitMQ kullanarak mesajı kuyrukla
            _rabbitMqService.SendMessage(senderId, messageVM.ReceiverId, newMessage.Content);

            // SignalR kullanarak mesajı alıcıya anında ilet
            await _hubContext.Clients.User(messageVM.ReceiverId).SendAsync("ReceiveMessage", senderId, newMessage.Content);

            // Göndericiye de mesajın iletilmesini sağla
            await _hubContext.Clients.User(senderId).SendAsync("ReceiveMessage", senderId, newMessage.Content);

            return RedirectToAction("Index", new { id = messageVM.ReceiverId });
        }

    }
}
