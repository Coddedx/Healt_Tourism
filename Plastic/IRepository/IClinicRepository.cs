using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetAllClinicsAsync(); //List<
        Task<Clinic?> GetByIdClinicAsync(int id); 
        Task<Franchise?> GetFranchiseByClinicId(int id); 
        List<Doctor?> GetDoctorByClinicId(int id); 
        List<Operation?> GetAllOperationByCategoryId(List<int> categoryId); //List<int> 
        List<Category?> GetAllCategories(); 
        IEnumerable<OperationDoctor?> GetOperationDoctor(int id); //yukardakiyle aynıydı normalde async
        bool IsDoctorObjectNull(DoctorViewModel _doctor);

    }
}
