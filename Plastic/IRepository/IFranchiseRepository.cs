using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IFranchiseRepository
    {
        Task<List<Franchise>> GetAllFranchisesAsync();
        Task<Franchise?> GetByIdFranchiseAsync(int id); //FranchiseViewModel
        Task AddAsync(FranchiseViewModel franchise);
        Task UpdateAsync(FranchiseViewModel franchise);
        Task DeleteAsync(int Id);
    }
}
