using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.IRepository
{
    public interface IDoctorRepository
    {
        //Task<List<_PartialDoctorViewModel>> GetAllDoctorsAsync();
        Task<DoctorViewModel?> MapNonNullProperties(ClinicModalViewModel doctorMVM);
        Task<Doctor?> GetDoctorByIdAsync(int id);
    }
}
