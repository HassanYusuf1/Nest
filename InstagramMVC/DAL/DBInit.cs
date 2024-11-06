using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InstagramMVC.Models;
using InstagramMVC.DAL;

namespace InstagramMVC.DAL;

public static class DBInit
{
    // This method seeds the database with initial data
    public static async Task SeedAsync(IApplicationBuilder app)
    {
        // Create a new scope to resolve services
        using var serviceScope = app.ApplicationServices.CreateScope();
        
        // Get the database context from the service provider
        var context = serviceScope.ServiceProvider.GetRequiredService<MediaDbContext>();
        
        // Get the user manager to handle user-related operations
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        // Delete the existing database and create a new one if needed
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Check if the default user already exists in the database
        var defaultUser = await userManager.FindByEmailAsync("Ali123@hotmail.com");
        if (defaultUser == null)
        {
            // Create a new default user if one doesn't already exist
            defaultUser = new IdentityUser
            {
                UserName = "Ali123@hotmail.com", 
                Email = "Ali123@hotmail.com", 
                EmailConfirmed = true 
            };
            // Add the user to the database with the specified password
            await userManager.CreateAsync(defaultUser, "Bekam2305."); // Use the provided password
        }

        // Seed data for images (Bilder)
        if (!context.Bilder.Any()) 
        {
            var bilder = new List<Bilde>
            {
                new Bilde
                {
                    Tittel = "Digg", 
                    Beskrivelse = "Va.", 
                    BildeUrl = "/images/Solnedgang_JPG.jpg", 
                    OpprettetDato = DateTime.Now.AddDays(-10), 
                    UserName = defaultUser.UserName 
                }
            };
            // Add the images to the database
            context.Bilder.AddRange(bilder);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for notes (Notater)
        if (!context.Notes.Any()) 
        {
            var notater = new List<Note>
            {
                new Note 
                { 
                    Tittel = "Dagbok - Dag 1", 
                    Innhold = "Startet dagen med en god frokost og dro på tur.", 
                    OpprettetDato = DateTime.Now.AddDays(-10), 
                    username = defaultUser.UserName 
                },
                new Note 
                { 
                    Tittel = "Dagbok - Dag 2", 
                    Innhold = "Møtte noen venner for fjelltur. Fantastisk utsikt!", 
                    OpprettetDato = DateTime.Now.AddDays(-9), 
                    username = defaultUser.UserName 
                }
            };
            
            // Add the notes to the database
            context.Notes.AddRange(notater);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for comments (Kommentarer) on images
        if (!context.Kommentarer.Any()) 
        {
            var kommentarer = new List<Kommentar>
            {
                new Kommentar { BildeId = 1, KommentarBeskrivelse = "Utrolig flott bilde!", KommentarTid = DateTime.Now.AddDays(-9) }, // Comment on the image
                new Kommentar { BildeId = 1, KommentarBeskrivelse = "Solnedgangen er magisk!", KommentarTid = DateTime.Now.AddDays(-8) } // Comment on the image
            };
            // Add the comments to the database
            context.Kommentarer.AddRange(kommentarer);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for comments on notes (Kommentarer for Notater)
        if (!context.Kommentarer.Any(k => k.NoteId == 1)) 
        {
            var noteComments = new List<Kommentar>
            {
                new Kommentar { NoteId = 1, KommentarBeskrivelse = "Dette er en flott start på dagen!", KommentarTid = DateTime.Now.AddDays(-5) }, 
                new Kommentar { NoteId = 1, KommentarBeskrivelse = "Høres ut som en fantastisk dag!", KommentarTid = DateTime.Now.AddDays(-4) } 
            };
            
            // Add the comments for notes to the database
            context.Kommentarer.AddRange(noteComments);
            await context.SaveChangesAsync(); // Save changes to the database
        }
    }
}
