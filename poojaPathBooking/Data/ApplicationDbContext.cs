namespace poojaPathBooking.Data;

using Microsoft.EntityFrameworkCore;
using poojaPathBooking.Models.Entities;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<PujaType> PujaTypes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PujaBooking> PujaBookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PujaType>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.Price)
                .HasDefaultValue(0.00M);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.ContactNumber).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<PujaBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.BookingStatus)
                .HasDefaultValue("Pending");

            entity.Property(e => e.IsPaid)
                .HasDefaultValue(false);

            entity.Property(e => e.Currency)
                .HasDefaultValue("INR");

            // Configure relationships
            entity.HasOne(e => e.PujaType)
                .WithMany()
                .HasForeignKey(e => e.PujaTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}