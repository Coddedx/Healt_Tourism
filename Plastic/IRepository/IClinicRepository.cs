using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetAllClinicsAsync(); //List<
        Task<Clinic?> GetByIdClinicAsync(string id); 
        Task<ClinicViewModel> SearchClinicsAndFranchises(string cityId, string districtId, string doctorName, string categoryId, string operationId); 
        Task<Franchise?> GetFranchiseByClinicId(string id); 
        List<Doctor?> GetDoctorByClinicId(string id); 
        List<Operation?> GetAllOperationByCategoryId(List<int> categoryId); //List<int> 
        List<Category?> GetAllCategories(); 
        IEnumerable<OperationDoctor?> GetOperationDoctor(string id); //yukardakiyle aynıydı normalde async
        bool IsDoctorObjectNull(DoctorViewModel _doctor);

    }
}
