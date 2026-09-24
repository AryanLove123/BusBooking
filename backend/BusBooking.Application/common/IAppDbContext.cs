using BusBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusBooking.Application.Common;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<BusOperator> BusOperators { get; }
    DbSet<Bus> Buses { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Passenger> Passengers { get; }
    ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}