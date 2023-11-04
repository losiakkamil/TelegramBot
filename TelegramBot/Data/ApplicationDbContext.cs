using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Medal> Medals { get; set; }
        public DbSet<UserMedal> UserMedals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserMedal>()
                .HasOne<User>(um => um.User)
                .WithMany(u => u.UserMedals)
                .HasForeignKey(um => um.UserId);

            modelBuilder.Entity<UserMedal>()
                .HasOne<Medal>(um => um.Medal)
                .WithMany(u => u.UserMedals)
                .HasForeignKey(um => um.MedalId);


            modelBuilder.Entity<Medal>().HasData(
                new Medal { Id = 1, ImagePath = "C:\\Users\\tefgo\\source\\repos\\TelegramBot\\TelegramBot\\images\\bronze.jpg", Name = "Bronze" },
                new Medal { Id = 2, ImagePath = "C:\\Users\\tefgo\\source\\repos\\TelegramBot\\TelegramBot\\images\\silver.jpg", Name = "Silver" },
                new Medal { Id = 3, ImagePath = "C:\\Users\\tefgo\\source\\repos\\TelegramBot\\TelegramBot\\images\\gold.jpg", Name = "Gold" },
                new Medal { Id = 4, ImagePath = "C:\\Users\\tefgo\\source\\repos\\TelegramBot\\TelegramBot\\images\\warn.jpg", Name = "Warn" }
                );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FuhrerBotDB;data source=DESKTOP-QN9FHUR;initial catalog=master;trusted_connection=true;");
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-QN9FHUR;Database=TelegramBotDB;trusted_connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True;encrypt=false");
                //IConfigurationRoot configuration = new ConfigurationBuilder()
                //    .SetBasePath(Directory.GetCurrentDirectory())
                //    .AddJsonFile("appsettings.json")
                //    .Build();
                //var connectionString = configuration.GetConnectionString("FuhrerBotDatabase");
                //optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
