using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SnowmobileLibrary.Data;

namespace SnowmobileWPF.Data
{
    public class SnowmobileContextFactory : IDesignTimeDbContextFactory<SnowmobileContext>
    {
        public SnowmobileContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SnowmobileContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SnowmobileDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

            return new SnowmobileContext(optionsBuilder.Options);
        }
    }
}