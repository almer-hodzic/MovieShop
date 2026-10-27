using Microsoft.Extensions.Configuration;

namespace Market.Infrastructure.Database.Seeders;

public static class PublicDemoSeeder
{
    public static async Task SeedAsync(DatabaseContext context, IConfiguration configuration)
    {
        var password = configuration["PublicDemo:Password"];
        if (string.IsNullOrWhiteSpace(password) || password.Length < 16)
            throw new InvalidOperationException("Set a unique public-demo password of at least 16 characters in secret storage.");
        const string username = "portfolio-demo";
        const string email = "demo@movieshop.invalid";
        var user = await context.Users.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Username == username || x.Email == email);
        if (user is not null && (user.IsAdmin || user.IsManager || user.IsEmployee ||
            user.Username != username || user.Email != email || user.IsDeleted))
            throw new InvalidOperationException("The public-demo identity must be a dedicated, active, non-staff account.");

        await DynamicDataSeeder.SeedCatalogAsync(context);
        if (user is not null)
            return; // Never reset an existing password, role, or security setting.
        user = new MarketUserEntity
        {
            Username = username, Email = email, Firstname = "Portfolio", Lastname = "Demo",
            IsAdmin = false, IsManager = false, IsEmployee = false, IsEnabled = true,
            IsEmailConfirmed = true, EmailConfirmedAtUtc = DateTime.UtcNow,
            IsTwoFactorEnabled = false
        };
        user.PasswordHash = new PasswordHasher<MarketUserEntity>().HashPassword(user, password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
