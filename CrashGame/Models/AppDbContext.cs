using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TestApp.Models;

namespace TestApp.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Game> CrashGames { get; set; }
        public DbSet<CrashGameResult> CrashGameResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            for (int i = 0; i < 5; i++)
            {
                var appUser = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = $"test{i}@test.com",
                    NormalizedEmail = $"test{i}@test.com".ToUpperInvariant(),
                    EmailConfirmed = true,
                    UserName = $"test{i}@test.com",
                    NormalizedUserName = $"test{i}@test.com".ToUpperInvariant(),
                    Balance = 100000
                };
                PasswordHasher<AppUser> ph = new();
                appUser.PasswordHash = ph.HashPassword(appUser, "Password123456&&&");

                builder.Entity<AppUser>().HasData(appUser);
            }
        }
    }
}
