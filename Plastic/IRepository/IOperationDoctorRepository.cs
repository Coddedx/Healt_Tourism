using Plastic.Models;

namespace Plastic.IRepository
{
    public interface IOperationDoctorRepository
    {
        Task<OperationDoctor?> GetOperationDoctorByIdAsync(int id);
    }
}
