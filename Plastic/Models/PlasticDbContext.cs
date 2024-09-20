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
            //modelBuilder.Entity<OperationUser>()
            //    .HasKey(c => new { c.OperationDoctorId, c.UserId });

            //modelBuilder.Entity<OperationDoctor>()
            //  .HasKey(c => new { c.OperationId, c.DoctorId });

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

            modelBuilder.Entity<Doctor>()
                    .HasOne(f => f.Franchise)
                    .WithMany(f => f.Doctors)
                    .HasForeignKey(f => f.FranchiseId);

            modelBuilder.Entity<Doctor>()
                    .HasOne(f => f.Clinic)
                    .WithMany(f => f.Doctors)
                    .HasForeignKey(f => f.ClinicId);

            modelBuilder.Entity<OperationDoctor>()
            .HasOne(od => od.Operation)
            .WithMany(o => o.OperationDoctors)
            .HasForeignKey(od => od.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OperationDoctor>()
                .HasOne(od => od.Doctor)
                .WithMany(d => d.OperationDoctors)
                .HasForeignKey(od => od.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OperationUser>()
                .HasOne(od => od.User)
                .WithMany(d => d.OperationUsers)
                .HasForeignKey(od => od.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<OperationUser>()
                .HasOne(od => od.OperationDoctor)
                .WithMany(d => d.OperationUsers)
                .HasForeignKey(od => od.OperationDoctorId)
                .OnDelete(DeleteBehavior.Restrict);



            //modelBuilder.Entity<OperationDoctor>()
            //		.HasOne(f => f.Doctor)
            //		.WithMany(f => f.OperationDoctors)
            //		.HasForeignKey(f => f.DoctorId)
            //                 .IsRequired()
            //                 .OnDelete(DeleteBehavior.Restrict); 

            //modelBuilder.Entity<OperationDoctor>()
            //    .HasOne(od => od.User)
            //    .WithMany(p => p.OperationDoctors)
            //    .HasForeignKey(od => od.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


    }
}
