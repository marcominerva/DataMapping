using DataMapping.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataMapping.DataAccessLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("People");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(30);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.City).WithMany(p => p.People)
                .HasForeignKey(e => e.CityId);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });
    }
}

