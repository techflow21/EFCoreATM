
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EFCoreATM_Data;

public class AtmDbContextFactory : IDesignTimeDbContextFactory<AtmDbContext>
{
    public AtmDbContextFactory() { }

    public AtmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AtmDbContext>();
        var connectionString = @"Data Source=DESKTOP-APMJTIG;Initial Catalog=efCoreAtmDB;Integrated Security=True;Encrypt=False";
        optionsBuilder.UseSqlServer(connectionString);

        return new AtmDbContext(optionsBuilder.Options);
    }
}
