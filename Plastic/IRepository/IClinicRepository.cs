using Plastic.Models;

namespace Plastic.IRepository
{
    public interface IClinicRepository
    {
        Task<Clinic?> GetByIdClinicAsync(int id); //FranchiseViewModel
        IQueryable<OperationDoctor?> GetOperationDoctor(int id); //yukardakiyle aynıydı normalde

    }
}
