using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Source.Models
{
    public class KvDbContext : DbContext
    {
        public DbSet<KV> KVs { get; set; }
        
        public KvDbContext(DbContextOptions<KvDbContext> o) : base(o) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<KV>()
                .ToTable("tblKVs")
                .HasKey(e => e.Key);
        }
    }
}