using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IFranchiseRepository
    {
        IQueryable<FranchiseViewModel> GetAllAsync();
        Task<Franchise> GetByIdAsync(int id); //FranchiseViewModel
        Task AddAsync(FranchiseViewModel franchise);
        Task UpdateAsync(FranchiseViewModel franchise);
        Task DeleteAsync(int Id);
    }
}
