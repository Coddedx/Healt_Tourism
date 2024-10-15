using Microsoft.AspNetCore.SignalR;
using Plastic.Models;
using Plastic.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Azure.Messaging;
using System.ComponentModel;


namespace Plastic.Hubs
{
    public class ChatHub : Hub
    {
        private readonly PlasticDbContext _context;
        private readonly RabbitMqService _rabbitMqService;
        public ChatHub(PlasticDbContext context, RabbitMqService rabbitMqService) //SONRADAN----------------------- CancellationToken cancellationToken 
        {
            _context = context;
            _rabbitMqService = rabbitMqService;

            // RabbitMQ dan mesajları dinlemeye başla
           // _rabbitMqService.ReceiveMessages(cancellationToken); //SONRADAN ----------------------- cancellationToken

            //SONRADAN --------------------------------
            //GetUnreadMessageCount(receiverId);
        }

        //Kullanıcı mesaj gönderdiğinde tetiklenir
        public async Task SendMessage(string receiverId, string messageContent,string data) //string message
        {
            Console.WriteLine("ChatHub a girdi: " + data);
            var senderId = Context.UserIdentifier; //UserIdentifier,ASP.NET Core Identity ile ilişkili bir kavramdır ve HttpContext.User nesnesinden veya SignalR'daki Hub sınıfı içindeki Context.UserIdentifier özelliğinden elde edilir o yüzden _context değil Context olmalı
            var newMessage = new Message
            {
                Content = messageContent, //message
                SenderId = senderId,
                ReceiverId = receiverId,
                SenAt = DateTime.UtcNow,
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Mesajın göndericiye ve alıcıya de iletilmesi SignalR ile 
           // await Clients.Users(new[] { receiverId, senderId }).SendAsync("ReceiveMessage", senderId, messageContent); //message tek kullanıcıya iletmek için User(receiverId).SendAsync("ReceiveMessage", senderId, message)
                                                                                                                       //ReceiveMessage -> SignalR'da sunucu, istemci tarafındaki bir metodu tetiklemek için SendAsync metodunu kullanır ve bu tetiklenen metodun adını sunucu tarafında belirler. Yani, ReceiveMessage adını tanımlarız ve bu isimle istemci tarafında(örneğin JavaScript veya.NET istemcisi) bir fonksiyonun bulunması gerekir. Sunucudan mesaj gönderildiğinde, istemci tarafındaki ReceiveMessage metodu çalıştırılacak ve parametreleri alacaktır.


            //SORADAN-----------------------------
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, messageContent); //, sentAt.ToString("g") //tek kullanıcıya iletmek için


            // Mesajı RabbitMQ'ya gönder
            _rabbitMqService.SendMessage(senderId, receiverId, messageContent);  //message

            // _rabbitMqService.SendMessage($"{senderId} says: {message} to {receiverId}"); //mesajların kuyrukta bekletilmesini sağlar. RabbitMqService ile mesajlar kuyrukta tutulabilir ve belirli olaylar olduğunda işlenebilir.

            // SONRADAN ----------------- Alıcıya kaç farklı kişiden mesaj geldiğini bildir
            // await GetUnreadMessageCount(receiverId);
        }

        // SONRADAN ------------------ Okunmamış mesajlar
        public async Task GetUnreadMessageCount() //string userId
        {
            Console.WriteLine("getunreadmessagecount içie girdi");
            var userId = Context.UserIdentifier;

            //receiver için okunmamış mesajların sayısını ve bu mesajları gönderen her bir kullanıcıdan kaç tane okunmamış mesaj aldığı (sender1 = 2 okunmamış mesaj, sender2 = 3 okunmamış mesaj gibi)
            var unreadMessages = _context.Messages
                .Where(m => m.ReceiverId == userId && m.Read == false)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, UnreadCount = g.Count() })
                .ToList();

            int unreadMessageCount = unreadMessages.Count; //toplamda kaç farklı gönderen kullanıcıdan okunmamış mesaj geldiğini (yani SenderId sayısını)

            // Bilgiyi kullanıcının istemcisine SignalR ile gönder
            await Clients.User(userId).SendAsync("ReceiveUnreadMessageCount", unreadMessageCount);
        }


    }
}
