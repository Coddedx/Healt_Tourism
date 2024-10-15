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
        public async Task<IActionResult> Index(string receiverId) //İD MESAJ AT DEDİĞİNDE GELEN CLİNİC/FRANCHİSEID(receiverId)!!!!!!!!!!!!  id receiver id,string SenderId,string ReceiverId string id,
        {
            Console.WriteLine("message controller ındexine geldi");
            string senderId = "";
            string senderUserId = "";
            string senderClinicId = "";
            string senderFranchiseId = "";
            string senderName = "Bilinmeyen???";

            using (var _context = new PlasticDbContext()) //connection hatası almamAk için sql bağlantılarını kont altında tutup using dışına çıkıldığında sql bağlantısını dispose eder
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("user"))
                    {
                        senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //claim, genellikle kullanıcının veritabanındaki Id alanına denk gelir GİRİŞ YAPAN HANGİ USER BULUNUYOR DİREK İD YE GEREK OLMADAN
                        senderId = senderUserId;
                        var senderUserr = _context.Users.FirstOrDefault(u => u.Id == senderId);
                        senderName = $"{senderUserr.FirstName}   {senderUserr.LastName}";
                    }
                    if (User.IsInRole("clinic"))
                    {
                        senderClinicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        senderId = senderClinicId;
                        var senderClinicc = _context.Clinics.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefault(a => a.Id == senderId);
                        senderName = senderClinicc.Name + "-" + senderClinicc.District.City.Name + "(" + senderClinicc.District.Name + ")";
                    }
                    if (User.IsInRole("franchise"))
                    {
                        senderFranchiseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        senderId = senderFranchiseId;
                        var senderFranchisee = _context.Franchises.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefault(a => a.Id == senderId);
                        senderName = senderFranchisee.Title + "-" + senderFranchisee.District.City.Name + "(" + senderFranchisee.District.Name + ")";
                    }

                }

                var messageVM = new MessageViewModel();

                //eski mesajlaşmalar
                var messages = _context.Messages
                    .Include(m => m.Sender)
                    .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId)) //id
                    .OrderBy(m => m.SenAt)
                    .ToList();

                var receiverUser = await _context.Users.FindAsync(receiverId);  //id
                var receiverClinic = await _context.Clinics.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefaultAsync(a => a.Id == receiverId);//;FindAsync(id)    //id
                var receiverFranchise = await _context.Franchises.Include(a => a.District).ThenInclude(a => a.City).FirstOrDefaultAsync(a => a.Id == receiverId);  //FindAsync(id)  //id

                //var receiverId = "";

                //var receiver = receiverUser;
                if (messages != null)
                {
                    messageVM.ReceiverId = receiverId;   //id
                    if (receiverUser != null)
                    {
                        messageVM.ReceiverName = receiverUser?.FirstName + " " + receiverUser?.LastName;
                        receiverId = receiverUser?.Id;
                    }
                    else if (receiverClinic != null)
                    {
                        messageVM.ReceiverName = receiverClinic?.Name + " " + receiverClinic?.District.City.Name + "-" + receiverClinic?.District.Name;
                        receiverId = receiverClinic?.Id;
                    }
                    else if (receiverFranchise != null)
                    { 
                        messageVM.ReceiverName = receiverFranchise?.Title + " " + receiverFranchise?.District.City.Name + "-" + receiverFranchise?.District.Name;
                        receiverId = receiverFranchise?.Id;
                    }
                    messageVM.SenderId = senderId;
                    messageVM.Messages = messages.Select(m => new MessageDto
                    {
                        SenderId = m.SenderId,
                        SenderName = senderName,//m.Sender != null ? m.Sender.User.FirstName + " " + m.Sender.User.LastName : "Bilinmeyen???", //sender ın null olup olmadığı kotnrol edilip değilse isim soy isim birleştirliyor
                                                // ReceiverName = messageVM.ReceiverName,
                        Content = m.Content,
                        SentAt = m.SenAt
                    }).ToList();
                }


                var messagesToUpdate = _context.Messages   //id
                    .Where(m => 
                    (m.SenderId == receiverId && m.ReceiverId == senderId) &&
                    m.Read == false) // Sadece okunmamış mesajlar  ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    .ToList();
                foreach (var message in messagesToUpdate)
                {
                    message.Read = true;
                }
                await _context.SaveChangesAsync();


                // MESAJ İKONUNA TIKLAYAN USER
                //senderıd ile uyuşan KONUŞMA GEÇMİŞİ OLAN KİŞİLERİ bul (receiverId lerini buluyoruz)(messages != null olmazsa döngü içine girmez receiverId boş string kalır ÇÖZ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!)
                var whoDidISpeakIds = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId != receiverId)  //mesaj attığı kişiden farklı olanları getiriyoruz != receiverId ile
                .Select(m => m.Receiver.Id) // Receiver'ın UserId'sini alıyoruz
                .Distinct() // Tekil değerler için
                .ToListAsync();

                messageVM.MessageDetails.UserDetails = await _context.Users
                                    .Where(m => whoDidISpeakIds.Contains(m.Id))
                                    .ToListAsync();

                messageVM.MessageDetails.ClinicDetails = await _context.Clinics
                                    .Include(m => m.District)
                                    .ThenInclude(m => m.City)
                                    .Where(m => whoDidISpeakIds.Contains(m.Id))
                                    .ToListAsync();

                messageVM.MessageDetails.FranchiseDetails = await _context.Franchises
                                    .Include(m => m.District)
                                    .ThenInclude(m => m.City)
                                    .Where(m => whoDidISpeakIds.Contains(m.Id))
                                    .ToListAsync();

                // MESAJ İKONUNA TIKLAYAN CLİNİC/FRANCHİSE
                if (User.IsInRole("clinic") || User.IsInRole("franchise")) //id 
                {
                    whoDidISpeakIds = await _context.Messages
                         .Include(m => m.Sender)
                         .Include(m => m.Receiver)
                         .Where(m => m.SenderId == senderId)  // || m.ReceiverId == senderId SenderId
                         .GroupBy(m => m.ReceiverId)
                         .Select(m => m.Key)
                         .ToListAsync();

                    messageVM.MessageDetails.UserDetails = await _context.Users
                        .Where(m => whoDidISpeakIds.Contains(m.Id))
                        .ToListAsync();

                    messageVM.MessageDetails.ClinicDetails = await _context.Clinics
                                        .Include(m => m.District)
                                        .ThenInclude(m => m.City)
                                        .Where(m => whoDidISpeakIds.Contains(m.Id))
                                        .ToListAsync();

                    messageVM.MessageDetails.FranchiseDetails = await _context.Franchises
                                        .Include(m => m.District)
                                        .ThenInclude(m => m.City)
                                        .Where(m => whoDidISpeakIds.Contains(m.Id))
                                        .ToListAsync();
                }

                return View(messageVM);
            }
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
                SenAt = DateTime.UtcNow,
                Read = false
            };

            // Mesajı veritabanına ekle
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // RabbitMQ kullanarak mesajı kuyrukla
            _rabbitMqService.SendMessage(senderId, messageVM.ReceiverId, newMessage.Content);

            // SignalR kullanarak mesajı alıcıya anında ilet
            await _hubContext.Clients.User(messageVM.ReceiverId).SendAsync("ReceiveMessage", senderId, newMessage.Content); //bu olduğunda mesaj 2 defa receiver kişisinde gözüküyor

            // Göndericiye de mesajın iletilmesini sağla
            await _hubContext.Clients.User(senderId).SendAsync("ReceiveMessage", senderId, newMessage.Content);

            return RedirectToAction("Index", new { receiverId = messageVM.ReceiverId });  //id
        }

    }
}
