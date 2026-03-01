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

            // Subscriber to Address (1-1)
            modelBuilder.Entity<Subscriber>()
                .HasOne(s => s.Address)
                .WithOne(a => a.SubscriberObject)
                .HasForeignKey<Address>(a => a.VSCA)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Subscription to Subscriber (1-1)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithOne(s => s.Subscription)
                .HasForeignKey<Subscriber>(s => s.VSCA)
                .OnDelete(DeleteBehavior.Restrict);

            // Email to Subscriber (1-many)
            modelBuilder.Entity<Email>()
                .HasOne(e => e.Subscriber)
                .WithMany()
                .HasForeignKey(e => e.VSCA)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum stored as string (more readable, safer long-term)
            modelBuilder.Entity<Subscription>()
                .Property(s => s.Source)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}