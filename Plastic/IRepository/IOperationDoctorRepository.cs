using Plastic.Models;

namespace Plastic.IRepository
{
    public interface IOperationDoctorRepository
    {
        List<OperationDoctor?> GetAllOperationDoctorByClinicId(int id); 
        List<OperationDoctor?> GetAllOperationDoctorByFranchiseId(int id);
        Task<OperationDoctor?> GetOperationDoctorByIdAsync(int id);

    }
}
