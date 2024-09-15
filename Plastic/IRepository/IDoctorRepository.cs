using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IDoctorRepository
    {
        //Task<List<_PartialDoctorViewModel>> GetAllDoctorsAsync();
        Task<DoctorViewModel?> MapNonNullProperties(ClinicViewModel doctorMVM);
        Task<Doctor?> GetDoctorByIdAsync(int id);
        List<Doctor?> GetDoctorsByNameAsync(string doctorName);
        IEnumerable<Doctor?> GetAllDoctorByClinicId(int id); 
        IEnumerable<Doctor?> GetAllDoctorByFranchiseId(int id); 

    }
}
