using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Plastic.Models
{
	public class PlasticDbContext : DbContext
	{
        public PlasticDbContext()
        {
                
        }

		public PlasticDbContext(DbContextOptions<PlasticDbContext> options) : base(options)
		{

		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<City> Cities { get; set; }
		public DbSet<Clinic> Clinics { get; set; }
		public DbSet<CommentClinic> CommentClinics { get; set; }
		public DbSet<CommentDoctor> CommentDoctors { get; set; }
		public DbSet<CommentFranchise> CommentFranchises { get; set; }
		public DbSet<CommentHospital> CommentHospitals { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<District> Districts { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Franchise> Franchises { get; set; }
		public DbSet<Hospital> Hospitals { get; set; }
		public DbSet<Operation> Operations { get; set; }
		public DbSet<OperationDoctor> OperationDoctors { get; set; }
		public DbSet<OperationUser> OperationUsers { get; set; }
		public DbSet<User> Users { get; set; }



		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //override onc yaz tab bas direk çıkıyo
		{
			optionsBuilder.UseSqlServer(
				"Server=LAPTOP-QT5SFJG4\\SQLEXPRESS; Database=PlasticDb; Trusted_Connection=true; Encrypt=false;");
			base.OnConfiguring(optionsBuilder);
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OperationUser>()
				.HasKey(c => new { c.OperationId, c.UserId });

			modelBuilder.Entity<OperationDoctor>()
			  .HasKey(c => new { c.OperationId, c.DoctorId });

            modelBuilder.Entity<CommentClinic>()
				.HasKey(c => new { c.UserId, c.ClinicId });
          
			modelBuilder.Entity<CommentDoctor>()
				.HasKey(c => new { c.UserId, c.DoctorId });
			
			modelBuilder.Entity<CommentFranchise>()
				.HasKey(c => new { c.UserId, c.FranchiseId });
			
			modelBuilder.Entity<CommentHospital>()
				.HasKey(c => new { c.UserId, c.HospitalId });


            base.OnModelCreating(modelBuilder);
		}


	}
}
