using Plastic.Models;

namespace Plastic.IRepository
{
    public interface IOperationDoctorRepository
    {
        List<OperationDoctor?> GetAllOperationDoctorByClinicId(string id); 
        List<OperationDoctor?> GetAllOperationDoctorByFranchiseId(string id);
        Task<OperationDoctor?> GetOperationDoctorByIdAsync(int id);

    }
}
