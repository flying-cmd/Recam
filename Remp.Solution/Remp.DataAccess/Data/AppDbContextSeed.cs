using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            var password = "Abc123$";

            // Seed PhotographyCompany user
            var photographyCompanyEmail = "photographycompany1@example.com";
            var photographyCompanyUser = await userManager.FindByEmailAsync(photographyCompanyEmail);

            if (photographyCompanyUser == null)
            {
                photographyCompanyUser = new User
                {
                    Email = photographyCompanyEmail,
                    UserName = photographyCompanyEmail
                };

                var createPhotographyCompanyResult = await userManager.CreateAsync(photographyCompanyUser, password);
                if (!createPhotographyCompanyResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to seed photography company user. {string.Join("; ", createPhotographyCompanyResult.Errors.Select(e => e.Description))}"
                    );
                }
            }

            if (!await userManager.IsInRoleAsync(photographyCompanyUser, RoleNames.PhotographyCompany))
            {
                var addPhotographyCompanyRoleResult = await userManager.AddToRoleAsync(photographyCompanyUser, RoleNames.PhotographyCompany);
                if (!addPhotographyCompanyRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign role '{RoleNames.PhotographyCompany}'. {string.Join("; ", addPhotographyCompanyRoleResult.Errors.Select(e => e.Description))}"
                    );
                }
            }

            if (!await context.PhotographyCompanies.AnyAsync(pc => pc.Id == photographyCompanyUser.Id))
            {
                await context.PhotographyCompanies.AddAsync(new PhotographyCompany
                {
                    Id = photographyCompanyUser.Id,
                    PhotographyCompanyName = "PhotographyCompany-1",
                });
            }

            // Seed Agent user
            var agentEmail = "agent1@example.com";
            var agentUser = await userManager.FindByEmailAsync(agentEmail);

            if (agentUser == null)
            {
                agentUser = new User
                {
                    Email = agentEmail,
                    UserName = agentEmail
                };

                var createAgentResult = await userManager.CreateAsync(agentUser, password);
                if (!createAgentResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to seed agent user. {string.Join("; ", createAgentResult.Errors.Select(e => e.Description))}"
                    );
                }
            }

            if (!await userManager.IsInRoleAsync(agentUser, RoleNames.Agent))
            {
                var addAgentRoleResult = await userManager.AddToRoleAsync(agentUser, RoleNames.Agent);
                if (!addAgentRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign role '{RoleNames.Agent}'. {string.Join("; ", addAgentRoleResult.Errors.Select(e => e.Description))}"
                    );
                }
            }

            if (!await context.Agents.AnyAsync(a => a.Id == agentUser.Id))
            {
                await context.Agents.AddAsync(new Agent
                {
                    Id = agentUser.Id,
                    AgentFirstName = "Agent",
                    AgentLastName = "One",
                    AvatarUrl = string.Empty,
                    CompanyName = "Test Agent"
                });
            }

            if (!await context.AgentPhotographyCompanies.AnyAsync(apc => apc.AgentId == agentUser.Id && apc.PhotographyCompanyId == photographyCompanyUser.Id))
            {
                await context.AgentPhotographyCompanies.AddAsync(new AgentPhotographyCompany
                {
                    AgentId = agentUser.Id,
                    PhotographyCompanyId = photographyCompanyUser.Id
                });
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
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
