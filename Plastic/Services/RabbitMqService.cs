using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Plastic.Hubs;
using Plastic.Models;
using Plastic.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace Plastic.Services
{
    public class RabbitMqService
    {
        //private readonly PlasticDbContext _context;
        // private readonly IServiceScopeFactory _scopeFactory;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IHubContext<ChatHub> _hubContext;

        public RabbitMqService(IHubContext<ChatHub> hubContext)//,PlasticDbContext context , IServiceScopeFactory scopeFactory
        {
            _hubContext = hubContext;
            //_context = context;
            //_scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.Uri = new Uri("amqps://jfujzfvi:2vANcOHyhYVd0ru7KghsSq8jW7GCfWD0@fish.rmq.cloudamqp.com/jfujzfvi");
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.QueueDeclare(queue: "chatQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            //QueueDeclare, RabbitMQ'da bir kuyruk oluşturur veya mevcut bir kuyruğa erişir. Bu, mesajların gönderileceği yeri tanımlar.
            _channel.QueueDeclare(
                      queue: "chatQueue",  // Kuyruk adı
                      durable: false,      // Kuyruğun kalıcı olup olmadığı (false: sunucu kapandığında silinir)
                      exclusive: false,    // Kuyruğun sadece bu bağlantı için özel olup olmadığı (false: herkese açık)
                      autoDelete: false,   // Kuyruk kullanılmadığında otomatik olarak silinir mi (false: silinmez)
                      arguments: null      // Kuyruk için ek ayarlar, genellikle null bırakılır
                                 );

        }

        //Mesajı RabbitMq kuyruğuna gönderme
        public void SendMessage(string senderId, string receiverId, string messageContent)
        {
            // Mesajı formatla
            var formattedMessage = $"{senderId}:{receiverId}:{messageContent}";
            var body = Encoding.UTF8.GetBytes(formattedMessage); //RabbitMQ, mesajları byte dizisi olarak alır, bu nedenle metni byte dizisine dönüştürmek gereklidir


            _channel.BasicPublish(exchange: "", routingKey: "chatQueue", basicProperties: null, body: body); //RabbitMQ'ya bir mesaj göndermek için kullanılır.
                                                                                                             //exchange ->hangi exchange'e (değişim noktası) gönderileceğini belirtir. "" bu da default exchange'i ifade eder. Default exchange, mesajı doğrudan routingKey ile belirtilen kuyruğa (queue) yönlendirir. Başka bir deyişle, mesaj bir exchange üzerinden geçmeden doğrudan kuyruğa gider.
                                                                                                             //routingKey-> mesajın RabbitMQ'daki chatQueue isimli kuyruğa gönderileceği anlamında. bir exchange olsaydı, routingKey mesajın hangi kurallara göre kuyruklara yönlendirileceğini belirlerdi.
                                                                                                             //basicProperties-> metadata bilgilerini içeren özellikleri belirler.Bu özellikler mesajın kalıcılığı, zaman aşımı gibi ek bilgileri içerebilir.
                                                                                                             //body-> byte dizisi (byte[]) türündedir ve RabbitMQ'ya gönderilen asıl mesaj.Mesaj metin tabanlıysa (örneğin JSON veya string), önce byte dizisine (byte[]) dönüştürülmesi gerekir.

        }

        //RabbitMq den mesajları alıp signalR ile ilet
        public void  ReceiveMessages() //CancellationToken cancellationToken Sonradan CancellationToken cancellationToken async Task ---------------------
        {
            //performansı artırmak için Her seferinde sadece bir mesaj alınır ve işlenir
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);

            string senderId = "";
            string receiverId = "";

            // Received olayına dinleyici ekleme
            consumer.Received += async (model, ea) =>
            {
                ////sonradan if hepsi
                //if (cancellationToken.IsCancellationRequested)
                //{
                //    _channel.BasicCancel(ea.ConsumerTag);  // İşlem iptal edildiyse consumer'ı durdur (channel ile gönderilen tag kullanarak)
                //    return;
                //}


                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                // Mesajı işle Mesajı alıcıya SignalR üzerinden gönder
                var parts = message.Split(':'); // Mesajı ayrıştır rabbitmq ya "{senderId}:{receiverId}:{messageContent}" formatında gönderilmişti
                if (parts.Length >= 3)
                {
                    senderId = parts[0].Trim(); // var 
                    receiverId = parts[1].Trim();
                    var content = parts[2].Trim();

                    //SONRADAN----------------------
                    try
                    {

                        //Mesajı SignalR ile göndeer
                        await _hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content);

                        //// Kullanıcının okunmamış mesaj sayısını güncelle
                        //await _hubContext.Clients.User(receiverId).SendAsync("GetUnreadMessageCount", receiverId);


                        ////SONRADAN --------------------
                        //// Mesaj başarıyla işlendiyse manuel onaylama
                        //_channel.BasicAck(ea.DeliveryTag, false);

                        Console.WriteLine($"Received message: {message}");
                    }
                    catch (Exception ex)
                    {
                        //// Eğer bir hata oluşursa mesajı işleme alınmadı olarak işaretleyin ve tekrar işlenmesini sağlayın
                        //_channel.BasicNack(ea.DeliveryTag, false, true);
                        Console.WriteLine($"Error processing message: {ex.Message}");

                    }
                }
                else
                {
                    Console.WriteLine("Received message is not in the expected format.");

                    //sonradan--------------
                    //_channel.BasicNack(ea.DeliveryTag, false, false); // Yanlış formatta mesajı işleme

                }
            };

            //SONRADAN VAR CONSUMERtAG = kısmı eklendi sadece gerisi vardı -------------------------- var consumerTag = 
            // Kuyruktan mesajları tüketmeye başla. autoAck: true olarak ayarlanmış durumda. Bu, mesajın otomatik olarak işlenmiş sayılacağı anlamına gelir,
            _channel.BasicConsume(queue: "chatQueue", autoAck: false, consumer: consumer);

            ////sonradan ----------------
            //// CancellationToken kullanarak işlem iptali sağlanır
            //await Task.Delay(Timeout.Infinite, cancellationToken);
            ////SONRADAN--------------------------------- if hepsi
            //if (cancellationToken.IsCancellationRequested)
            //{
            //    // İşlem iptal edildiyse, consumer'ı durdurmak için consumerTag kullan
            //    _channel.BasicCancel(consumerTag);
            //}

        }


    }
}
