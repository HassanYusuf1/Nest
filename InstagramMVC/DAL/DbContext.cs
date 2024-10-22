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

    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
    }
}

