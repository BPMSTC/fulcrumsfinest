using Microsoft.EntityFrameworkCore;
using SnowmobileLibrary.Models;

namespace SnowmobileLibrary.Data
{
    public class SnowmobileContext : DbContext
    {
        public SnowmobileContext(DbContextOptions<SnowmobileContext> options)
            : base(options)
        {
        }

        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Email> Emails => Set<Email>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Subscriber PK
            modelBuilder.Entity<Subscriber>()
                .HasKey(s => s.VSCA);

            // Address to Subscriber (1-many)
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Subscriber)
                .WithMany()
                .HasForeignKey(a => a.VSCA)
                .OnDelete(DeleteBehavior.Cascade);

            // Subscription to Subscriber (1-many)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithMany()
                .HasForeignKey(s => s.VSCA)
                .OnDelete(DeleteBehavior.Cascade);

            // Email to Subscriber (1-many)
            modelBuilder.Entity<Email>()
                .HasOne(e => e.Subscriber)
                .WithMany()
                .HasForeignKey(e => e.VSCA)
                .OnDelete(DeleteBehavior.Cascade);

            // Enum stored as string (more readable, safer long-term)
            modelBuilder.Entity<Subscription>()
                .Property(s => s.Source)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}