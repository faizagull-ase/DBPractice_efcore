using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ClinicFlowDemo;

// Lets `dotnet ef migrations add` / `dotnet ef database update` construct
// ClinicFlowDbContext at design time, now that it needs IOptions<...> instead
// of a parameterless constructor. Reads the same appsettings.json the app uses.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ClinicFlowDbContext>
{
    public ClinicFlowDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionStrings = new ConnectionStringsOptions();
        configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

        return new ClinicFlowDbContext(Options.Create(connectionStrings));
    }
}
