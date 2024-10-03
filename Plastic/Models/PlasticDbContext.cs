using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Cryptography;


namespace Plastic.Models
{
    public class PlasticDbContext : IdentityDbContext<AppUser>// DbContext
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
        public DbSet<Message> Messages { get; set; }


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
                .HasKey(c => new { c.AppUserId, c.ClinicId });

            modelBuilder.Entity<CommentDoctor>()
                .HasKey(c => new { c.AppUserId, c.DoctorId });

            modelBuilder.Entity<CommentFranchise>()
                .HasKey(c => new { c.AppUserId, c.FranchiseId });

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
                .HasOne(od => od.AppUser)
                .WithMany(d => d.OperationUsers)
                .HasForeignKey(od => od.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OperationUser>()
                .HasOne(od => od.OperationDoctor)
                .WithMany(d => d.OperationUsers)
                .HasForeignKey(od => od.OperationDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sender ilişkisini yapılandırma
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver ilişkisini yapılandırma
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<AppUser>()
            //    .HasOne(a => a.User)
            //    .WithMany(u => u.Users)
            //    .HasForeignKey(a => a.UserId);




            // 1:1 ilişki ayarı
            modelBuilder.Entity<AppUser>() //1:1 ilişki
                .HasOne(a => a.User) // AppUser'ın bir User'ı olacak
                .WithOne() // User'ın bir AppUser'ı olacak
                .HasForeignKey<User>(u => u.Id); // User tablosundaki Id, AppUser.Id ile eşleşecek                 

            modelBuilder.Entity<AppUser>() //1:1 ilişki
                .HasOne(a => a.Clinic)
                .WithOne()
                .HasForeignKey<Clinic>(u => u.Id)
                .OnDelete(DeleteBehavior.NoAction);         //Özellikle, CommentClinic tablosunda hem AppUserId hem de ClinicId ile ilişkili yabancı anahtarlar var. Bu iki yabancı anahtar aynı tabloda başka kısıtlamalarla çakışabilir ve SQL Server bu durumda döngüsel ya da çakışan cascade delete yollarına izin vermez.
                                                            //Yabancı anahtarların silme işlemlerinde ON DELETE NO ACTION veya ON DELETE SET NULL gibi bir işlem belirtilmesi gerekir. Bu, silme işlemi sırasında döngü veya çakışmayı önler.

            modelBuilder.Entity<AppUser>() //1:1 ilişki
                .HasOne(a => a.Franchise)
                .WithOne()
                .HasForeignKey<Franchise>(u => u.Id)
                .OnDelete(DeleteBehavior.NoAction);


            //modelBuilder.Entity<AppUser>() // 1:1 ilişki
            //     .HasOne(a => a.Franchise) // AppUser'ın bir Franchise'ı olacak
            //     .WithOne() // Franchise'ın bir AppUser'ı olacak
            //     .HasForeignKey<AppUser>(u => u.FranchiseId) // AppUser tablosundaki FranchiseId, Franchise.Id ile eşleşecek
            //     .OnDelete(DeleteBehavior.NoAction);



            base.OnModelCreating(modelBuilder);
        }


    }
}
