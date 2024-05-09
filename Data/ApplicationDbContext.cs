using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using Katastar.Models;

namespace Katastar.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Predmet> Predmeti { get; set; }
        public DbSet<Arhiva> Arhiva { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Predmet>()
            .Property(p => p.Id)
            .ValueGeneratedNever();

            builder.Entity<Arhiva>()
            .Property(p => p.Id)
            .ValueGeneratedNever();


            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }
    }

    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

        }
    }
}