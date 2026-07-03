using Microsoft.EntityFrameworkCore;

public class AppDbContext: DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>()
        .HasIndex( u => u.Email)
        .IsUnique();

        modelBuilder.Entity<User>()
        .Property( u => u.IsActive)
        .HasDefaultValue(true);

        modelBuilder.Entity<Address>()
        .HasOne( a => a.User)
        .WithMany( u => u.Addresses)
        .HasForeignKey ( a => a.UserId);

        modelBuilder.Entity<Currency>()
        .HasIndex( c => c.Code)
        .IsUnique();
    }


} 