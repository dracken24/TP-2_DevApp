using Microsoft.EntityFrameworkCore;
using TP_2.Models;

namespace TP_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RendezVous> RendezVous { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ApplicationBureau.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration du modèle User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Nom).HasMaxLength(100);
                entity.Property(e => e.Prenom).HasMaxLength(100);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Index unique sur Username et Email
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuration du modèle RendezVous
            modelBuilder.Entity<RendezVous>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.DateDebut).IsRequired();
                entity.Property(e => e.DateFin).IsRequired();
                entity.Property(e => e.Lieu).HasMaxLength(100);
                entity.Property(e => e.Client).HasMaxLength(100);
                entity.Property(e => e.Statut).HasMaxLength(20);
                entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Relation avec User (cascade delete)
                entity.HasOne(e => e.User)
                      .WithMany(u => u.RendezVous)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
