using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api
{
    /// <summary>
    /// Contexte de base de données pour l'application.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Table des livres.
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Table des emprunts.
        /// </summary>
        public DbSet<Loan> Loans { get; set; }

        /// <summary>
        /// Table des notifications.
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Configuration des relations entre les entités.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation ApplicationUser -> Loan
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Loans)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade

            // Relation ApplicationUser -> Notification
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Book -> Loan
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Loans)
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

    
