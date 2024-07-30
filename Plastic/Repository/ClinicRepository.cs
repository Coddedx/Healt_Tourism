using Microsoft.EntityFrameworkCore;
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

        public async Task<Clinic?> GetByIdClinic(int id) //Clinic
        {
            return await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<OperationDoctor?> GetOperationDoctor(int id)
        {            
            //var franchise = _context.Franchises
                     //    .Include(a => a.Doctors)
                     //    .Where(c => c.ClinicId == id).ToList();  //where ile sorgulama ıqueryable döndürür!!! ToList diyerek liste olarak döndürürüz, FirstOrDefault diyerek tek bir dönüt bekliyosak kullanab. ınclude dersek ihtiyacımız olan bağlantılı tabloları da dahil edebiliriz.

            //var doctor = _context.Doctors.Find(id);

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

        public bool IsDoctorObjectNull(DoctorViewModel _doctor) //2 den fazla veri doluysa ture gönderiyor
        {
            if (_doctor == null)
                return false;

            int filledPropertiesCount = 0;

            foreach (PropertyInfo property in _doctor.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(_doctor);
                if (value != null && !(value is string str && string.IsNullOrEmpty(str)))
                {
                    filledPropertiesCount++;
                    if (filledPropertiesCount >= 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
