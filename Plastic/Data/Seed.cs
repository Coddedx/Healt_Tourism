using Microsoft.AspNetCore.Identity;
using Plastic.Models;

namespace Plastic.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PlasticDbContext>();
                context.Database.EnsureCreated();
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.Developer))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Developer));
                if (!await roleManager.RoleExistsAsync(UserRoles.Clinic))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Clinic));
                if (!await roleManager.RoleExistsAsync(UserRoles.Franchise))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Franchise));

                //Users
                //Uygulamaya yeni kullanıcılar eklemek. Var olan kullanıcılara roller atamak.Kullanıcı bilgilerini doğrulamak veya parolasını değiştirmek. için userManager kullanılır yapmıcaksan gerek yok 
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            }
        }

    }
}
