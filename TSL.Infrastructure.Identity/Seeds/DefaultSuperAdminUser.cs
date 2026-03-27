using Microsoft.AspNetCore.Identity;
using TSL.Core.Domain.Enums;
using TSL.Infrastructure.Identity.Entities;

namespace TSL.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@tls.com",
                FirstName = "Maui",
                LastName = "Jaquez",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if(userManager.Users.All(u => u.Email != defaultUser.Email)) 
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null) 
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());

                }
            }

        }
    }
}
