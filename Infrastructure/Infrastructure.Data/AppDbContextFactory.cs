using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql
            (
                "Server=localhost;Database=DbJogoFiap;User=root;Password=root;",
                new MySqlServerVersion(new Version(8, 0, 42))
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
