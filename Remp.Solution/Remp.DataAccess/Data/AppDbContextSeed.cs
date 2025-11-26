using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Remp.Models.Constants;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public static class AppDbContextSeed
{
    public static async Task SeedRolesAsyc(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var roles = new[] { RoleNames.PhotographyCompany, RoleNames.Agent };

        // Transaction
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed users
            // Seed PhotographyCompany user
            var email = "photographycompany1@example.com";
            var password = "Abc123$";

            if (await userManager.FindByNameAsync(email) == null)
            {
                var photographyCompanyUser = new User { Email = email, UserName = email };

                // Add PhotographyCompany user
                var result = await userManager.CreateAsync(photographyCompanyUser, password);

                if (result.Succeeded)
                {
                    // Add PhotographyCompany role
                    await userManager.AddToRoleAsync(photographyCompanyUser, RoleNames.PhotographyCompany);

                    // Add PhotographyCompany to PhtographyCompany table
                    await context.PhotographyCompanies.AddAsync(new PhotographyCompany
                    {
                        Id = photographyCompanyUser.Id,
                        PhotographyCompanyName = "PhotographyCompany-1",
                    });

                    await context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }
}
