using ex_EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace ex_EntityFrameworkCore.DBContext
{
    public class EFCoreDbContext : DbContext
    {

        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options)
        : base(options) // The base(options) call passes the options to the base DbContext class constructor.
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine,LogLevel.Information);

            optionsBuilder.UseSqlServer(@"Server=Hiren;Database=ex_EntityFrameworkCore;Trusted_Connection=True;TrustServerCertificate=True;");
            
        }
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Branch> Branches { get; set; }
    }

}
