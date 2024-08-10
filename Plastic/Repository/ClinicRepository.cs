﻿using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
using System.Reflection;

namespace Plastic.Repository
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly PlasticDbContext _context;

        public ClinicRepository(PlasticDbContext context) //constructor
        {
            _context = context; //new PlasticDbContext();
        }
        public async Task<List<Clinic>> GetAllClinicsAsync()   //List<
        {
            return await _context.Clinics.Include(a => a.District).ToListAsync();
        }
        public async Task<Clinic?> GetByIdClinicAsync(int id) //Clinic
        {
            return await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Franchise?> GetFranchiseByClinicId(int id)
        {
            throw new NotImplementedException();
        }

        //  ???????????????????  YANLIŞ !!!!!!!!!!!!!!!!!!!!!!!!!!1
        public IQueryable<OperationDoctor?> GetOperationDoctor(int id)
        {
            //var franchise = _context.Franchises
            //    .Include(a => a.Doctors)
            //    .Where(c => c.ClinicId == id).ToList();  //where ile sorgulama ıqueryable döndürür!!! ToList diyerek liste olarak döndürürüz, FirstOrDefault diyerek tek bir dönüt bekliyosak kullanab. ınclude dersek ihtiyacımız olan bağlantılı tabloları da dahil edebiliriz.

            var doctor = _context.Doctors.Include(c => c.Franchise).ThenInclude(t => t.ClinicId);

            var clinic = _context.Clinics.FirstOrDefault(c => c.Id == id);

            //return await _context.OperationDoctors
            //     .Include(a => a.Franchises)
            //     .Include(b => b.Clinics)
            //     .Where(c => c.DoctorId == doctor.Id && doctor.FranchiseId == franchise.Id && franchise.ClinicId == id);                


            //include  yapmazsam view da kullanamıyorum bu verileri!!!
            var operationDoctor = _context.OperationDoctors
                .Include(a => a.Operation)
                .Include(b => b.Doctor)
                    .Where(d => clinic.Id == id);
            return operationDoctor;
            //           await _context.OperationDoctors
            //           .Include(od => od.Doctor)
            //                   .ThenInclude(d => d.Franchise)
            //                        .ThenInclude(f => f.Clinic)
            //.Where(c => c.DoctorId == doctor.Id && doctor.FranchiseId == franchise.Id && franchise.ClinicId == id);      // _context => _context.DoctorId == id 

        }


        //   !!!!!!!!!!!!!!!!!!!!!!!!!!!GEREK KALMADI!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public bool IsDoctorObjectNull(DoctorViewModel _doctor) //2 den fazla veri doluysa ture gönderiyor. REFLECTİON yapıyoruz
        {
            if (_doctor == null)
                return false;

            int filledPropertiesCount = 0;

            foreach (PropertyInfo property in _doctor.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //GetType->nesnenin hangi sınıfa ait olduğunu,     GetProperties-> belirtilen türün özelliklerini (properties) alır
            //BindingFlags.Public bayrağı-> yalnızca genel (public) özelliklerin alınmasını,    BindingFlags.Instance bayrağı-> yalnızca örnek (instance) özelliklerin alınmasını (statik olmayan özelliklerin alınması)
            {
                var value = property.GetValue(_doctor);
                if (value != null && !(value is string str && string.IsNullOrEmpty(str)))
                //value is string str->value değişkeninin bir string olup olmadığını kont.ed. value string ise, bu string str değişkenine atanır.
                {
                    filledPropertiesCount++;
                    if (filledPropertiesCount >= 2)
                    {
                        return true;
                    }
                }
            }
            return false;

            // CONTROLLER DA KULLANIRKAN BÖYLE KULLANIYORUZ !!!!!!!!!!!!1
            //bool ısDoctorVMNull =  _clinicRepository.IsDoctorObjectNull(_doctorVM);
            //if (ısDoctorVMNull == true && (_doctorVM.Doctor.Status != true || _doctorVM.Doctor.Status == null)) //Doctor. eğer doctorvm içi doluysa yani yanlış yazılan doktor formunun verileri geri geldiyse clinicvm in doktorunu dolduralım ki düzeltebilsin baştan yazmasın ve doktorun formdan girilen verileri doğruysa status u true olcağı için girilen verilerin tekrardan yeni açılan formda gözükmemesi için status kontrolü yapıyoruz
            //{
            //    clinicVM.Doctor.FirstName = _doctorVM.Doctor.FirstName;//??????????????
            //}

            //TempData["DoctorViewModel"] = JsonSerializer.Serialize(doctorVM); //JsonConvert kullandığımda cs0103 hatası alıyoruz onun yerine JsonSerializer kullandık
        }

        public Task<Doctor?> GetDoctorByClinicId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
