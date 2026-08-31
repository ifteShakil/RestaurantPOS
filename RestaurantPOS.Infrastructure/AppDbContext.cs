using RestaurantPOS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace RestaurantPOS.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemVariant> MenuItemVariants => Set<MenuItemVariant>();
    public DbSet<MenuItemBranchPrice> MenuItemBranchPrices => Set<MenuItemBranchPrice>();
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentMethodType> PaymentMethodTypes => Set<PaymentMethodType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // decimal precision
        modelBuilder.Entity<Branch>().Property(b => b.TaxRatePercent).HasPrecision(5, 2);
        modelBuilder.Entity<MenuItem>().Property(m => m.BasePrice).HasPrecision(10, 2);
        modelBuilder.Entity<MenuItemVariant>().Property(m => m.ExtraPrice).HasPrecision(10, 2);
        modelBuilder.Entity<MenuItemBranchPrice>().Property(m => m.Price).HasPrecision(10, 2);
        modelBuilder.Entity<OrderItem>().Property(m => m.UnitPrice).HasPrecision(10, 2);
        modelBuilder.Entity<Bill>().Property(m => m.SubTotal).HasPrecision(10, 2);
        modelBuilder.Entity<Bill>().Property(m => m.TaxAmount).HasPrecision(10, 2);
        modelBuilder.Entity<Bill>().Property(m => m.DiscountAmount).HasPrecision(10, 2);
        modelBuilder.Entity<Bill>().Property(m => m.TotalAmount).HasPrecision(10, 2);
        modelBuilder.Entity<Bill>().Property(b => b.TaxRatePercent).HasPrecision(5, 2);
        modelBuilder.Entity<Payment>().Property(m => m.Amount).HasPrecision(10, 2);


        // Unique constraints
        modelBuilder.Entity<Branch>().HasIndex(b => b.Code).IsUnique();
        modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<PaymentMethodType>().HasIndex(p => p.Code).IsUnique();

        // একটি MenuItem-এর জন্য একটি Branch-এ শুধু একটাই price override থাকতে পারবে
        modelBuilder.Entity<MenuItemBranchPrice>()
            .HasIndex(x => new { x.MenuItemId, x.BranchId })
            .IsUnique();

        // Cascade delete বন্ধ রাখা জরুরি — একটা Branch ডিলিট হলে যেন পুরো Order history 
        // accidentally মুছে না যায়
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Branch)
            .WithMany(b => b.Orders)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .OnDelete(DeleteBehavior.Restrict);


        // Seed Data
        modelBuilder.Entity<PaymentMethodType>().HasData(
            new PaymentMethodType { Id = 1, Name = "Cash", Code = "CASH", RequiresTransactionRef = false, IsActive = true },
            new PaymentMethodType { Id = 2, Name = "Card", Code = "CARD", RequiresTransactionRef = true, IsActive = true },
            new PaymentMethodType { Id = 3, Name = "MTN Mobile Money", Code = "MTN_MOMO", RequiresTransactionRef = true, CountryCode = "SZ", IsActive = true }
        );


    }
}
