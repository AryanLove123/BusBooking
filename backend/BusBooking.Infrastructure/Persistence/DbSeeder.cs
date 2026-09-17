using BusBooking.Application.Interfaces;
using BusBooking.Domain.Enums;
using BusBooking.Domain.Models;
using BusBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher)
    {
        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync()) return;

        var admin = new User
        {
            Name = "System Admin",
            Email = "admin@example.com",
            PasswordHash = hasher.Hash("Admin@123"),
            Role = UserRole.Admin
        };

        var demoUser = new User
        {
            Name = "Demo User",
            Email = "user@example.com",
            PasswordHash = hasher.Hash("User@1234"),
            Role = UserRole.User
        };

        db.Users.AddRange(admin, demoUser);

        var operatorEntity = new BusOperator
        {
            Name = "Uttarakhand Travels",
            ContactEmail = "ops@uktravels.example",
            ContactPhone = "+91-9000000000"
        };

        db.BusOperators.Add(operatorEntity);
        await db.SaveChangesAsync();
        var today = DateTime.UtcNow.Date.AddDays(1);
        
        var buses = new List<Bus>
        {
            new Bus
            {
                BusOperatorId = operatorEntity.Id,
                BusNumber = "UK-101",
                BusType = "AC Sleeper",
                Source = "Delhi",
                Destination = "Dehradun",
                DepartureUtc = today.AddHours(22),
                ArrivalUtc = today.AddHours(29),
                TotalSeats = 40,
                AvailableSeats = 40,
                FarePerSeat = 850.00m
            },
            new Bus
            {
                BusOperatorId = operatorEntity.Id,
                BusNumber = "UK-102",
                BusType = "Non-AC Seater",
                Source = "Delhi",
                Destination = "Dehradun",
                DepartureUtc = today.AddHours(7),
                ArrivalUtc = today.AddHours(13),
                TotalSeats = 45,
                AvailableSeats = 45,
                FarePerSeat = 550.00m
            },
            new Bus
            {
                BusOperatorId = operatorEntity.Id,
                BusNumber = "UK-201",
                BusType = "AC Seater",
                Source = "Dehradun",
                Destination = "Delhi",
                DepartureUtc = today.AddHours(20),
                ArrivalUtc = today.AddHours(26),
                TotalSeats = 40,
                AvailableSeats = 40,
                FarePerSeat = 800.00m
            },
            new Bus
            {
                BusOperatorId = operatorEntity.Id,
                BusNumber = "UK-301",
                BusType = "AC Sleeper",
                Source = "Delhi",
                Destination = "Manali",
                DepartureUtc = today.AddDays(1).AddHours(21),
                ArrivalUtc = today.AddDays(2).AddHours(10),
                TotalSeats = 36,
                AvailableSeats = 36,
                FarePerSeat = 1200.00m
            },
            new Bus
            {
                BusOperatorId = operatorEntity.Id,
                BusNumber = "UK-401",
                BusType = "Non-AC Seater",
                Source = "Delhi",
                Destination = "Jaipur",
                DepartureUtc = today.AddHours(6),
                ArrivalUtc = today.AddHours(11),
                TotalSeats = 50,
                AvailableSeats = 2,
                FarePerSeat = 450.00m
            }
        };

        db.Buses.AddRange(buses);
        await db.SaveChangesAsync();
    }
}