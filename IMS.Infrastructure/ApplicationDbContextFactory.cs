using IMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IMS.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Try to read from configuration first
            string connectionString;
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("../IMS.Web/appsettings.json", optional: true)
                    .Build();

                connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            catch
            {
                // Fallback to default connection string with SQL Server Express
                connectionString = "Server=ADMIN\\SQLEXPRESS;Database=ansar_ims;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False";
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fixed: Removed duplicate "Server=" text
                connectionString = "Server=ADMIN\\SQLEXPRESS;Database=ansar_ims;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False";
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}