using ex_ConceptOfMVC.Models.Authontication;
using Microsoft.EntityFrameworkCore;

namespace ex_ConceptOfMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure usernames are unique
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
