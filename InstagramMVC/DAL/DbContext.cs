using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InstagramMVC.Models;
using Microsoft.Extensions.Options;

namespace InstagramMVC.DAL
{

    public class MediaDbContext : IdentityDbContext<IdentityUser>
    {
        public MediaDbContext (DbContextOptions<MediaDbContext> options) : base (options)
        {
            //Database.EnsureCreated();

        }
    
        public DbSet<Bilde> Bilder {get; set;}
        public DbSet<Note> Notes {get; set;}
        public DbSet<Kommentar> Kommentarer { get; set; }  
    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the relationship between Kommentar and Note
        modelBuilder.Entity<Kommentar>()
            .HasOne(k => k.Note)
            .WithMany(n => n.Kommentarer) // Assuming Note has a collection of Kommentar
            .HasForeignKey(k => k.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Kommentar and Bilde
        modelBuilder.Entity<Kommentar>()
            .HasOne(k => k.Bilde)
            .WithMany(b => b.Kommentarer) // Assuming Bilde has a collection of Kommentar
            .HasForeignKey(k => k.BildeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    }
}

