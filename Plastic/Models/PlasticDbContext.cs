using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Cryptography;

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
		//public DbSet<CommentHospital> CommentHospitals { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<District> Districts { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Franchise> Franchises { get; set; }
		//public DbSet<Hospital> Hospitals { get; set; }
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

			//modelBuilder.Entity<CommentHospital>()
			//	.HasKey(c => new { c.UserId, c.HospitalId });

			modelBuilder.Entity<Clinic>()
					.HasMany(c => c.Franchises)
					.WithOne(f => f.Clinic)
					 .HasForeignKey(f => f.ClinicId);
			
			modelBuilder.Entity<District>()
					.HasMany(c => c.Clinics)
					.WithOne(f => f.District)
					.HasForeignKey(f => f.DistrictId)
                    .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<District>()
					.HasMany(c => c.Franchises)
					.WithOne(f => f.District)
					.HasForeignKey(f => f.DistrictId)
				    .OnDelete(DeleteBehavior.Restrict); /* Parent kaydının silinmesi durumunda ilgili Child kayıtlarının silinmesini engeller.Bu durumda, eğer Parent kaydını silmeye çalışırsanız ve ilişkili Child kayıtları varsa, veritabanı bir hata fırlatır ve silme işlemi engellenir.*/

     //       modelBuilder.Entity<Franchise>()
     //.HasOne(f => f.District)
     //.WithMany(f => f.Franchises) // Bir District birden fazla Franchise ile ilişkilendirilebilir, ancak Franchise tek bir District'e bağlıdır.
     //.HasForeignKey(f => f.DistrictId);

            modelBuilder.Entity<Doctor>()
					.HasOne(f => f.Franchise)
					.WithMany(f => f.Doctors)
					.HasForeignKey(f => f.FranchiseId);

            modelBuilder.Entity<Doctor>()
					.HasOne(f => f.Clinic)
					.WithMany(f => f.Doctors)
					.HasForeignKey(f => f.ClinicId);

            base.OnModelCreating(modelBuilder);
		}


	}
}
