using Enhanzer_API.Locations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer_API.Locations.Persistence;

public sealed class LocationsDbContext(DbContextOptions<LocationsDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var location = modelBuilder.Entity<Location>();
        location.ToTable("Locations", "dbo");
        location.HasKey(item => item.LocationCode);
        location.Property(item => item.LocationCode)
            .HasColumnName("Location_Code")
            .HasMaxLength(100)
            .IsUnicode()
            .IsRequired();
        location.Property(item => item.LocationName)
            .HasColumnName("Location_Name")
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();
        location.Property(item => item.StockHandle)
            .HasColumnName("Stock_Handle")
            .IsRequired();
        location.Property(item => item.Address)
            .HasColumnName("Address")
            .HasMaxLength(1000)
            .IsUnicode()
            .IsRequired();
        location.Property(item => item.Phone)
            .HasColumnName("Phone")
            .HasMaxLength(50)
            .IsUnicode()
            .IsRequired();
        location.Property(item => item.Status)
            .HasColumnName("Status")
            .IsRequired();
    }
}
