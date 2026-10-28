using Microsoft.EntityFrameworkCore.Design;
namespace Market.Infrastructure.Database;

public sealed class DeploymentContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args) => new(
        new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql().Options, TimeProvider.System);
}
