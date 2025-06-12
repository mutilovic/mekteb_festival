using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mekteb_Festival.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Registration> Registrations => Set<Registration>();
        public DbSet<Takmicar> Takmicari => Set<Takmicar>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Registration>()
                .HasMany(r => r.Takmicari)
                .WithOne(t => t.Registration)
                .HasForeignKey(t => t.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
