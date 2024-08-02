﻿using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
using System.Reflection;
using System.Text.Json;

namespace Plastic.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        //   !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!GEREK KALMADI!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public async Task<DoctorViewModel?> MapNonNullProperties(ClinicModalViewModel doctorMVM) //Reflection yöntemiyle
        {
            var doctorVM = new DoctorViewModel // bul olmadığında doctor dan obje üretemiyorum???
            {
                Doctor = new Doctor()
            };
            //sadece null olmayan verileri tekrar gönderelim ki formu yeniden kaldıkları yerden döndürebilsinler
            ////source(gelen)->>doctorMVM,  destination(gidicek)->> doctorVM
            //// Source ve Destination türlerini al
            var sourceType = doctorMVM.GetType();
            var destinationType = doctorVM.GetType();

            // Source türündeki tüm genel ve örnek özellikleri al
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Source property değerini al
                var value = property.GetValue(doctorMVM);

                // Eğer değer null değilse, Destination nesnesindeki ilgili property'e ata
                if (value != null)
                {
                    var destProperty = destinationType.GetProperty(property.Name);
                    if (destProperty != null && destProperty.CanWrite)
                    {
                        destProperty.SetValue(doctorVM, value);
                    }
                }
            }

            // TEMPDATA ÇALIŞMIYO ÇÖZ!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Doctor özelliğinin null gidiyor clinic controllera. modelin RedirectToAction yöntemiyle gönderilirken Doctor nesnesinin seri hale getirilip geri yüklenmemesi
            //RedirectToAction yöntemi, genellikle URL parametreleri aracılığıyla veri gönderir ve karmaşık nesneleri(örneğin Doctor gibi) düzgün bir şekilde seri hale getiremez.
            //Bu sorunu çözmek için TempData veya Session kullanarak karmaşık veri nesnelerini geçici olarak depolayabiliriz.
            // DoctorViewModel'i TempData'ya serialize edip sakla
            //TempData["DoctorViewModel"] = JsonSerializer.Serialize(doctorVM); //JsonConvert kullandığımda cs0103 hatası alıyoruz onun yerine JsonSerializer kullandık

            return doctorVM;
        }
    }
}