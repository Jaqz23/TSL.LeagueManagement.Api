using Microsoft.AspNetCore.Identity;
using TSL.Core.Domain.Enums;
using TSL.Infrastructure.Identity.Entities;

namespace TSL.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {

            if (!await roleManager.RoleExistsAsync(Roles.Admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }

            if (!await roleManager.RoleExistsAsync(Roles.User.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

        }
    }
}
