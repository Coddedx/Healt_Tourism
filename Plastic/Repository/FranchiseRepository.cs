using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;

namespace Plastic.Repository
{
    public class FranchiseRepository : IFranchiseRepository
    {
        private readonly PlasticDbContext _context;

        public FranchiseRepository(PlasticDbContext context) //constructor
        {
            _context = context; //new PlasticDbContext();
        }

        public async Task AddAsync(FranchiseViewModel franchise)
        {
            var newfranchise = new FranchiseViewModel();
            //{
            //    Id = franchise.Id,
            //    //HospiatlId = franchise.HospiatlId,
            //    ClinicId = franchise.ClinicId,
            //    DistrictId = franchise.DistrictId,
            //    Title = franchise.Title,
            //    Description = franchise.Description,
            //    CertificationNumber = franchise.CertificationNumber,
            //    Adress = franchise.Adress,
            //    ImageUrl = franchise.ImageUrl,
            //    Email = franchise.Email,
            //    Phone = franchise.Phone,
            //    InstagramUrl = franchise.InstagramUrl,
            //    Status = franchise.Status,
            //    CreatedDate = franchise.CreatedDate,
            //    CreatedBy = franchise.CreatedBy,
            //    UpdatedDate = franchise.UpdatedDate,
            //    UpdatedBy = franchise.UpdatedBy,
            //    Deleted = franchise.Deleted,
            //};
            franchise = newfranchise; //??????????????????????????

            //await _context.Franchises.AddAsync(newfranchise);  //newfranchise de hata veriyor??????????????????*
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int Id)
        {
            Franchise franchise = await _context.Franchises.FindAsync(Id);
            _context.Franchises.Remove(franchise);
            await _context.SaveChangesAsync();
        }

    
        public async Task<List<Franchise>> GetAllFranchisesAsync() //public IQueryable<FranchiseViewModel> GetAllAsync()
        {

            return await _context.Franchises.Include(a => a.District).ToListAsync();

            //var franchise = _context.Franchises
            //           .Select(e => new FranchiseViewModel
            //           {
            //               Id = e.Id,
            //               //HospiatlId = e.HospitalId,
            //               ClinicId = e.ClinicId,
            //               DistrictId = e.DistrictId,
            //               Title = e.Title,
            //               Description = e.Description,
            //               CertificationNumber = e.CertificationNumber,
            //               Address = e.Address,
            //               ImageUrl = e.ImageUrl,
            //               Email = e.Email,
            //               Phone = e.Phone,
            //               InstagramUrl = e.InstagramUrl,
            //               Status = e.Status,
            //               CreatedDate = e.CreatedDate,
            //               CreatedBy = e.CreatedBy,
            //               UpdatedDate = e.UpdatedDate,
            //               UpdatedBy = e.UpdatedBy,
            //               Deleted = e.Deleted
            //           });
            //return franchise;
        }

        public async Task<Franchise?> GetByIdFranchiseAsync(int id) //FranchiseViewModel
        {
           // return await _context.Franchises.FirstOrDefaultAsync(e => e.Id == id);
            //return await _context.Franchises.Where(c => c.Id == id).AsNoTracking().FirstOrDefaultAsync();
            //return await _context.Franchises.Include(i => i.Clinic).FirstOrDefaultAsync(i => i.Id == id);
             return await _context.Franchises
                //.Include(i => i.Hospital)
                //.Include(i => i.Clinic)
                .Include(i => i.District)
                .ThenInclude(i => i.City)
                .FirstOrDefaultAsync(c => c.Id == id);  
            //  FindAsync(id)(bunu kullanırken include kullanılamıyor????)  
        }

        public async Task UpdateAsync(FranchiseViewModel franchiseUpdated)
        {
            var franchise = await _context.Franchises.FindAsync(franchiseUpdated.Id);
           
            franchise.Id = franchiseUpdated.Id;
            //franchise.HospitalId = franchiseUpdated.HospiatlId;
            franchise.ClinicId = franchiseUpdated.ClinicId;
            franchise.DistrictId = franchiseUpdated.DistrictId;
            franchise.Title = franchiseUpdated.Title;
            franchise.Description = franchiseUpdated.Description;
            franchise.CertificationNumber = franchiseUpdated.CertificationNumber;
            franchise.Adress = franchiseUpdated.Adress;
            franchise.ImageUrl = franchiseUpdated.ImageUrl;
            franchise.Email = franchiseUpdated.Email;
            franchise.Phone = franchiseUpdated.Phone;
            franchise.InstagramUrl = franchiseUpdated.InstagramUrl;
            franchise.Status = franchiseUpdated.Status;
            franchise.CreatedDate = franchiseUpdated.CreatedDate;
            franchise.CreatedBy = franchiseUpdated.CreatedBy;
            franchise.UpdatedDate = franchiseUpdated.UpdatedDate;
            franchise.UpdatedBy = franchiseUpdated.UpdatedBy;
            franchise.Deleted = franchiseUpdated.Deleted;
            
            _context.Franchises.Update(franchise);
            await _context.SaveChangesAsync();
        }


    }
}
