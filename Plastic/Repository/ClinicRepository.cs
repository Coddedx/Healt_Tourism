﻿using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
using System.Reflection;
using System.Security.Cryptography;

namespace Plastic.Repository
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly PlasticDbContext _context;
        private readonly IDoctorRepository _doctorRepository;


        public ClinicRepository(PlasticDbContext context, IDoctorRepository doctorRepository) //constructor
        {
            _context = context; //new PlasticDbContext();
            _doctorRepository = doctorRepository;
        }
        public async Task<List<Clinic>> GetAllClinicsAsync()   //List<
        {
            return await _context.Clinics.Include(a => a.District).Where(a => a.Status == true && a.Deleted == false).ToListAsync();
        }
        public async Task<Clinic?> GetByIdClinicAsync(string id) //Clinic
        {
            return await _context.Clinics.
                Include(a => a.District).
                ThenInclude(i => i.City).
                FirstOrDefaultAsync(c => c.Id == id); 
        }

        public Task<Franchise?> GetFranchiseByClinicId(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OperationDoctor?> GetOperationDoctor(string id)  //IQueryable olduğunda tolist yapamayız çünkü sorgu döndürür
        {
            //var franchise = _context.Franchises
            //    .Include(a => a.Doctors)
            //    .Where(c => c.ClinicId == id).ToList();  //where ile sorgulama ıqueryable döndürür!!! ToList diyerek liste olarak döndürürüz, FirstOrDefault diyerek tek bir dönüt bekliyosak kullanab. ınclude dersek ihtiyacımız olan bağlantılı tabloları da dahil edebiliriz.

            var doctor = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == id).ToList();
            var doctorIds = doctor.Select(a => a.Id).ToList();
            //var clinic = _context.Clinics.FirstOrDefault(c => c.Id == id);

            //return await _context.OperationDoctors
            //     .Include(a => a.Franchises)
            //     .Include(b => b.Clinics)
            //     .Where(c => c.DoctorId == doctor.Id && doctor.FranchiseId == franchise.Id && franchise.ClinicId == id);                


            //include  yapmazsam view da kullanamıyorum bu verileri!!!
            var operationDoctor = _context.OperationDoctors
                .Include(a => a.Operation)
                .Include(b => b.Doctor)
                    .Where(d => doctorIds.Contains(d.DoctorId) ).ToList();
            return operationDoctor;
            //           await _context.OperationDoctors
            //           .Include(od => od.Doctor)
            //                   .ThenInclude(d => d.Franchise)
            //                        .ThenInclude(f => f.Clinic)
            //.Where(c => c.DoctorId == doctor.Id && doctor.FranchiseId == franchise.Id && franchise.ClinicId == id);      // _context => _context.DoctorId == id 

        }
        public List<Doctor?> GetDoctorByClinicId(string id)
        {
            //var clinic = _context.Clinics
            //    .Where(c => c.Id == _id).ToList();

            //var franchiseAll = _context.Franchises.ToList();


            //var franchise = _context.Franchises
            //        .Include(c => c.Clinic)
            //        //.Where(_context => _context.ClinicId == _id).ToList();
            //        .Where(a => a.ClinicId == _id).ToList();
            //if (franchise == null || !franchise.Any())
            //{
            //    // Veri bulunamadı, loglama veya hata yönetimi.
            //}

            try
            {
                var doctor = _context.Doctors
                             .Where(d => d.ClinicId == id).ToList();
                return doctor;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Operation?> GetAllOperationByCategoryId(List<int> categoryId) //List<int> 
        {
            try
            {
                return  _context.Operations
                    .Where(a => categoryId.Contains(a.CategoryId)) //   == a.Id
                    .ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Category?> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        //   !!!!!!!!!!!!!!!!!!!!!!!!!!!GEREK KALMADI!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public bool IsDoctorObjectNull(DoctorViewModel _doctor) //2 den fazla veri doluysa ture gönderiyor (REFLECTİON)
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

        public async Task<ClinicViewModel> SearchClinicsAndFranchises(string cityId, string districtId, string doctorName, string categoryId, string operationId)
        {
            //await _clinicRepository.GetAllClinicsAsync() ?? await _franchiseRepository.GetAllFranchisesAsync() ??
            var ClinicFranchiseVM = new ClinicViewModel()
            {
                Clinics =  new List<Clinic>(), 
                Franchises =  new List<Franchise>()
            };

            if (!string.IsNullOrEmpty(cityId) && string.IsNullOrEmpty(districtId))
            {
                ClinicFranchiseVM.Clinics = _context.Clinics
                    .Where(c => c.District.City.Id == Convert.ToInt32(cityId)).ToList();
                ClinicFranchiseVM.Franchises = _context.Franchises
                    .Where(f => f.District.City.Id == Convert.ToInt32(cityId)).ToList();
            }

            if (!string.IsNullOrEmpty(districtId))
            {
                ClinicFranchiseVM.Clinics = _context.Clinics
                    .Where(c => c.District.Id.ToString() == districtId).ToList();
                ClinicFranchiseVM.Franchises = _context.Franchises
                    .Where(f => f.District.Id.ToString() == districtId).ToList();
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                var doctors = _doctorRepository.GetDoctorsByNameAsync(doctorName);
                
                //var clinicIds = doctors
                //    .Where(d => Convert.ToInt16(d.ClinicId).HasValue)
                //    .Select(d => d.ClinicId.Value).Distinct().ToList();

                var clinicIds = doctors
                    .Where(doctors => !string.IsNullOrEmpty(doctors.ClinicId))
                    .Select(d => d.ClinicId).Distinct().ToList();
                
                //var franchiseIds = doctors
                //    .Where(d => d.FranchiseId.HasValue)
                //    .Select(d => d.FranchiseId.Value).Distinct().ToList();

                var franchiseIds = doctors
                    .Where(d => !string.IsNullOrEmpty(d.FranchiseId))
                    .Select(d => d.FranchiseId).Distinct().ToList();

                ClinicFranchiseVM.Clinics = _context.Clinics
                    .Where(c => clinicIds.Contains(c.Id)).ToList();
                ClinicFranchiseVM.Franchises = _context.Franchises
                    .Where(f => franchiseIds.Contains(f.Id)).ToList();
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                var operationDoctorIds = _context.OperationDoctors
                                            .Where(a => a.Operation.CategoryId == Convert.ToInt32(categoryId))
                                             .Select(a => a.DoctorId).ToList();
                
                ClinicFranchiseVM.Clinics = _context.Clinics
                                            .Where(c => c.Doctors.Any(d => operationDoctorIds.Contains(d.Id))).ToList();
                ClinicFranchiseVM.Franchises = _context.Franchises
                                            .Where(c => c.Doctors.Any(d => operationDoctorIds.Contains(d.Id))).ToList();
            }
            if (!string.IsNullOrEmpty(operationId))
            {
                var operationDoctorIds = _context.OperationDoctors
                        .Where(a => a.Operation.Id == Convert.ToInt32(operationId))
                         .Select(a => a.DoctorId).ToList();
               
                ClinicFranchiseVM.Clinics = _context.Clinics
                    .Where(c => c.Doctors.Any(d => operationDoctorIds.Contains(d.Id))).ToList();
                ClinicFranchiseVM.Franchises = _context.Franchises
                    .Where(f => f.Doctors.Any(d => operationDoctorIds.Contains(d.Id))).ToList();
            }
            return ClinicFranchiseVM;
        }


    }
}
