using System.Security.Cryptography.X509Certificates;
using BusBooking.Application.Common;
using BusBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BusBooking.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){ }

    public DbSet<User> Users => Set<User>();
    public DbSet<BusOperator> BusOperators => Set<BusOperator>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => await Database.BeginTransactionAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<BusOperator>(e =>
        {
            e.ToTable("BusOperators");
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);
            e.Property(x => x.ContactEmail).HasMaxLength(150);
            e.Property(x => x.ContactPhone).HasMaxLength(30);
        });

        modelBuilder.Entity<Bus>(e =>
        {
            e.ToTable("Buses");
            e.Property(x => x.BusNumber).IsRequired().HasMaxLength(30);
            e.Property(x => x.BusType).IsRequired().HasMaxLength(50);
            e.Property(x => x.Source).IsRequired().HasMaxLength(100);
            e.Property(x => x.Destination).IsRequired().HasMaxLength(100);
            e.Property(x => x.FarePerSeat).HasColumnType("decimal(10,2)");
            e.Property(x => x.RowVersion).IsRowVersion();

            e.HasOne(x => x.BusOperator)
                .WithMany(o => o.Buses)
                .HasForeignKey(x => x.BusOperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.Source, x.Destination, x.DepartureUtc });
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("Bookings");
            e.Property(x => x.TotalFare).HasColumnType("decimal(10,2)");
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            e.HasOne(x => x.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Bus)
                .WithMany(b => b.Bookings)
                .HasForeignKey(x => x.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<Passenger>(e =>
        {
            e.ToTable("Passengers");
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Gender).HasConversion<string>().HasMaxLength(10);

            e.HasOne(x => x.Booking)
                .WithMany(b => b.Passengers)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}