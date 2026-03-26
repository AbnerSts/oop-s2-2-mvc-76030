using Microsoft.AspNetCore.Identity;

namespace FoodSafety.mvc.Data
{
    public static class SeedRoles
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Inspector" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // COMMON PASSWORD
            string password = "Admin123!";

            string adminEmail = "admin@test.com";
            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "Admin");
            }

            string inspectorEmail = "inspector@test.com";
            var inspector = await userManager.FindByEmailAsync(inspectorEmail);

            if (inspector == null)
            {
                inspector = new IdentityUser
                {
                    UserName = inspectorEmail,
                    Email = inspectorEmail
                };

                await userManager.CreateAsync(inspector, password);
                await userManager.AddToRoleAsync(inspector, "Inspector");
            }

        }
    }
}