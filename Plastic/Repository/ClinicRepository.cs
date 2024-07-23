using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;

namespace Plastic.Repository
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly PlasticDbContext _context;

        public ClinicRepository(PlasticDbContext context) //constructor
        {
            _context = context; //new PlasticDbContext();
        }

        public async Task<Clinic?>  GetByIdClinicAsync(int id)
        {
            return await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<OperationDoctor?> GetOperationDoctor(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault();
            var franchise = _context.Franchises.Where(c => c.ClinicId == id);

            var operation = _context.OperationDoctors.Where(c => c.DoctorId == doctor.Id);
            return operation;
        }
    }
}
