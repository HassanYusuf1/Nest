using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using InstagramMVC.Models;
using Microsoft.Extensions.Options;

namespace InstagramMVC.DAL
{

    public class MediaDbContext : IdentityDbContext
    {
        public MediaDbContext (DbContextOptions<MediaDbContext> options) : base (options)
        {
            //Database.EnsureCreated();

        }
    

    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
    }
}

