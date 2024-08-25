using Plastic.IRepository;
using Plastic.Models;

namespace Plastic.Repository
{
    public class OperationDoctorRepository : IOperationDoctorRepository
    {
        public Task<OperationDoctor?> GetOperationDoctorByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
