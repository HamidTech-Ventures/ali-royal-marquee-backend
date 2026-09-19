using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AliRoyalMarquee.Infrastructure.Persistence;

public static class AppDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AppDbSeeder");
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        try
        {
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            await SeedRolesAndPermissionsAsync(context);
            await SeedAdminUserAsync(context, configuration, passwordHasher);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static async Task SeedRolesAndPermissionsAsync(AppDbContext context)
    {
        var roles = new[]
        {
            "Owner", "Administrator", "Manager", "Accounts", 
            "BookingStaff", "EventCoordinator", "Storekeeper", "KitchenManager", "Staff"
        };

        var permissions = new[]
        {
            "View", "Create", "Edit", "Delete", "Approve", "Export"
        };

        foreach (var p in permissions)
        {
            if (!await context.Permissions.AnyAsync(x => x.Name == p))
            {
                context.Permissions.Add(new Permission { Name = p });
            }
        }
        await context.SaveChangesAsync();

        foreach (var r in roles)
        {
            if (!await context.Roles.AnyAsync(x => x.Name == r))
            {
                context.Roles.Add(new Role { Name = r });
            }
        }
        await context.SaveChangesAsync();

        var ownerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Owner");
        if (ownerRole != null)
        {
            var allPermissions = await context.Permissions.ToListAsync();
            foreach (var perm in allPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == ownerRole.Id && rp.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = ownerRole.Id, PermissionId = perm.Id });
                }
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedAdminUserAsync(AppDbContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        var adminEmail = "admin@codepispor.com";
        var adminPassword = configuration["AdminPassword"];

        if (string.IsNullOrEmpty(adminPassword))
        {
            throw new InvalidOperationException("AdminPassword must be configured in environment or appsettings.");
        }

        if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var ownerRole = await context.Roles.FirstAsync(r => r.Name == "Owner");

            var adminUser = new User
            {
                FullName = "System Administrator",
                Email = adminEmail,
                Status = UserStatus.Active,
                RoleId = ownerRole.Id
            };
            
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
