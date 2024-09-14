using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;

namespace Plastic.Repository
{
    public class OperationDoctorRepository : IOperationDoctorRepository
    {
        private readonly PlasticDbContext _context;
        public OperationDoctorRepository(PlasticDbContext context)
        {
            _context = context;
        }
        public List<OperationDoctor?> GetAllOperationDoctorByClinicId(int id)
        { 
            var doctor = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == id).ToList();
            var doctorIds = doctor.Select(a => a.Id).ToList();
            var operationDoctor = _context.OperationDoctors
                .Include(a => a.Operation)
                .Include(b => b.Doctor)
                    .Where(d => doctorIds.Contains(d.DoctorId)).ToList();
            return operationDoctor;
        }
        public List<OperationDoctor?> GetAllOperationDoctorByFranchiseId(int id)
        {
            var doctor = _context.Doctors.Include(a => a.Clinic).Where(b => b.FranchiseId == id).ToList();
            var doctorIds = doctor.Select(a => a.Id).ToList();
            var operationDoctor = _context.OperationDoctors
                .Include(a => a.Operation)
                .Include(b => b.Doctor)
                    .Where(d => doctorIds.Contains(d.DoctorId)).ToList();
            return operationDoctor;
        }

        public async Task<OperationDoctor?> GetOperationDoctorByIdAsync(int id)
        {
            return await _context.OperationDoctors.FindAsync(id);
        }
    }
}
