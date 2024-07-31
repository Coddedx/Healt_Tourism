using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Plastic.IRepository;
using Plastic.Helper;

namespace Plastic.Services
{
    public class PhotoService:IPhotoService
    {
        //CLOUDİNARY DEĞİŞİKLİKLERİ->>>>>><APPSETTİNGJSON CLOUDİNARYSETTİNGS PROGRAM->2 PHOTOSERVİCE IPHOTOSERVİCE

        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config) //this option is going to allow us bring in instances(örnek) an once we have that we can bring in our cloudinary
        {
            //be able to utilize our account
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(acc); //its gonna automaticlly create the acoount for us 
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            //whenever we add new file we are going to have uploud result 
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                //cloudinary gives you ability to trim and edit photos and zoom in on faces exc
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                //await ---> çalıştıracağı task’in işini bitirmesini beklerken diğer işlemlerin asenkron olarak devam etmesini sağlayan özelliği yani non-blocking olarak çalışmayı sağlar( sadece Task dönen durumlarda kullanılabilir). await operatörü kullandığımız Task hata fırlattığında try/catch bloğu içinde yakalama imkanı var
                uploadResult = await _cloudinary.UploadAsync(uploadParams); //we are gonna take uploadresult that we initialized and the whatever cloudinary return so the cloudinary handle the actual upload so we are gonna passing those upload params 
            }
            return uploadResult; //her şey iyi giderse upload result ı döndrüyüoruz
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId); //ıphotoservice de publicId string i alıyor
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }

    }
}
