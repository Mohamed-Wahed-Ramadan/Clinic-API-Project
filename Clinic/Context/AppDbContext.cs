using Microsoft.EntityFrameworkCore;
using Clinic.Models;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).HasMaxLength(50);

                entity.HasIndex(u => u.Name).IsUnique();
            });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "d0ct0r77",
                    Password = "AQAAAAIAAYagAAAAENOKhTr/Qe2VdOp6rgecouojG4J7OVhGdgIReTls23KfJS7NWoSFksSOXPehhFfd1A==", //asd123!@# 
                    Role = "Admin",
                    Phone = "(+20)1123002663",
                    FullName = "DOCTOR"
                },
                new User
                {
                    Id = 2,
                    Name = "n4r3e77",
                    Password = "AQAAAAIAAYagAAAAENOKhTr/Qe2VdOp6rgecouojG4J7OVhGdgIReTls23KfJS7NWoSFksSOXPehhFfd1A==", //asd123!@#
                    Role = "Admin",
                    Phone = "(+20)1123002663",
                    FullName = "NURSE"
                },
                new User
                {
                    Id = 3,
                    Name = "@ss5st4nt77",
                    Password = "AQAAAAIAAYagAAAAENOKhTr/Qe2VdOp6rgecouojG4J7OVhGdgIReTls23KfJS7NWoSFksSOXPehhFfd1A==", //asd123!@#
                    Role = "Admin",
                    Phone = "(+20)1123002663",
                    FullName = "ASSESTANCE"
                }
            );

        }
    }
}