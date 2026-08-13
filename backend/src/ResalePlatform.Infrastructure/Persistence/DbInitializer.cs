using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResalePlatform.Domain.Entities;
using ResalePlatform.Infrastructure.Identity;

namespace ResalePlatform.Infrastructure.Persistence;

/// <summary>
/// Применяет миграции и наполняет БД начальными данными:
/// роли (User/Admin) и учётную запись администратора.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { "User", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var config = provider.GetRequiredService<IConfiguration>();
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@resale.local";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin123$";

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                DisplayName = "Администратор",
                EmailConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        await SeedCategoriesAsync(db);
    }

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync())
            return;

        // (Название, slug, [подкатегории: (Название, slug)])
        var tree = new (string Name, string Slug, (string Name, string Slug)[] Children)[]
        {
            ("Электроника", "electronics", new[]
            {
                ("Телефоны", "phones"), ("Ноутбуки", "laptops"), ("Телевизоры", "tv"),
            }),
            ("Транспорт", "transport", new[]
            {
                ("Автомобили", "cars"), ("Запчасти", "auto-parts"),
            }),
            ("Недвижимость", "realty", new[]
            {
                ("Квартиры", "apartments"), ("Дома", "houses"),
            }),
            ("Личные вещи", "personal", new[]
            {
                ("Одежда", "clothing"), ("Обувь", "shoes"),
            }),
            ("Дом и сад", "home-garden", new[]
            {
                ("Мебель", "furniture"), ("Бытовая техника", "appliances"),
            }),
        };

        var order = 0;
        foreach (var (name, slug, children) in tree)
        {
            var parent = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Slug = slug,
                SortOrder = order++,
            };
            db.Categories.Add(parent);

            var childOrder = 0;
            foreach (var (childName, childSlug) in children)
            {
                db.Categories.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = childName,
                    Slug = childSlug,
                    ParentId = parent.Id,
                    SortOrder = childOrder++,
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
