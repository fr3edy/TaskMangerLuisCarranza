using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data;

public static class DataSeeder
{
    public static void Initialize(ApplicationDbContext context)
    {

        context.Database.Migrate();


        if (context.Users.Any()) return;

        string hashedDemoPassword = BCrypt.Net.BCrypt.HashPassword("Admin123!");

        var demoUser = new User(
            Guid.NewGuid(), 
            "demo@test.com",
            hashedDemoPassword, 
            "Admin Demo" 
        );

        context.Users.Add(demoUser);

        var demoTask = new TaskItem(
            Guid.NewGuid(),
            "Finish .NET Interview Exercise",
            "Complete the Clean Architecture, TDD and API implementation.",
            DateTime.UtcNow.AddDays(2),
            demoUser.Id
        );

        context.Tasks.Add(demoTask);

        context.SaveChanges();
    }
}