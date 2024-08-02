using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IClinicRepository
    {
        Task<Clinic?> GetByIdClinic(int id); //FranchiseViewModel
        Task<Franchise?> GetFranchiseByClinicId(int id); //FranchiseViewModel
        IQueryable<OperationDoctor?> GetOperationDoctor(int id); //yukardakiyle aynıydı normalde async
        bool IsDoctorObjectNull(DoctorViewModel _doctor);

    }
}
