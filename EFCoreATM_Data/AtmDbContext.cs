
using Azure;
using EFCoreATM_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreATM_Data;

public class AtmDbContext : DbContext
{
    public AtmDbContext(DbContextOptions<AtmDbContext> options) : base(options) { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<TransactionDetail> Transactions { get; set; }
    public DbSet<AtmMachine> AtmMachine { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(e =>
        {
            e.Property(p => p.UserName)
                .IsRequired()
                .HasMaxLength(20);
            e.HasIndex(p => p.UserName, $"IX_{nameof(Admin)}_{nameof(Admin.UserName)}")
                .IsUnique();
        });


        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(p => new { p.Email, p.PhoneNumber })
                .IsRequired()
                .HasMaxLength(50);
            e.HasIndex(p => new { p.Email, p.PhoneNumber },
                    $"IX_Unique_{nameof(Customer.Email)}{nameof(Customer.PhoneNumber)}")
                .IsUnique();
        });


        base.OnModelCreating(modelBuilder);
    }
}
