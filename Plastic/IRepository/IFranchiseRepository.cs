using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IFranchiseRepository
    {
        IQueryable<FranchiseViewModel> GetAllAsync();
        Task<Franchise> GetByIdAsync(int id); //FranchiseViewModel
        Task<Clinic> GetByIdClinicAsync(int id); //FranchiseViewModel
        IQueryable<OperationDoctor> GetOperationDoctor(int id); //yukardakiyle aynıydı normalde async
        Task AddAsync(FranchiseViewModel franchise);
        Task UpdateAsync(FranchiseViewModel franchise);
        Task DeleteAsync(int Id);
    }
}
