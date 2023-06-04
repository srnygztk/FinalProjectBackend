using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastracture.Persistence.Contexts;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));

        optionsBuilder.UseMySql("Server=141.98.112.67;Port=7002;Database=serenay_gozutok;Uid=serenay_gozutok;Pwd=d88aoLYtw0DWYkices3Ad5Gxz;", serverVersion);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}