using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GrpcServer.Source.Models
{
    public class KvDbContextFactory : IDesignTimeDbContextFactory<KvDbContext>
    {
        public KvDbContext CreateDbContext(string[] args)
        {
            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var optionsBuilder = new DbContextOptionsBuilder<KvDbContext>().UseSqlite(conf.GetConnectionString("DBCS"));
            return new KvDbContext(optionsBuilder.Options);
        }
    }
}
