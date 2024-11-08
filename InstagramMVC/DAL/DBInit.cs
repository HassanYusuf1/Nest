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

        // Seed data for images (Pictures)
        if (!context.Pictures.Any()) 
        {
            var pictures = new List<Picture>
            {
                new Picture
                {
                    Title = "Digg", 
                    Description = "Va.", 
                    PictureUrl = "/images/Solnedgang_JPG.jpg", 
                    UploadDate = DateTime.Now.AddDays(-10), 
                    UserName = defaultUser.UserName 
                }
            };
            // Add the images to the database
            context.Pictures.AddRange(pictures);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for notes (Notes)
        if (!context.Notes.Any()) 
        {
            var notes = new List<Note>
            {
                new Note 
                { 
                    Title = "Dagbok - Dag 1", 
                    Content = "Startet dagen med en god frokost og dro på tur.", 
                    UploadDate = DateTime.Now.AddDays(-10), 
                    username = defaultUser.UserName 
                },
                new Note 
                { 
                    Title = "Dagbok - Dag 2", 
                    Content = "Møtte noen venner for fjelltur. Fantastisk utsikt!", 
                    UploadDate = DateTime.Now.AddDays(-9), 
                    username = defaultUser.UserName 
                }
            };
            
            // Add the notes to the database
            context.Notes.AddRange(notes);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for comments (Comments) on images
        if (!context.Comments.Any()) 
        {
            var comments = new List<Comment>
            {
                new Comment { PictureId = 1, CommentDescription = "Amazing picture!", CommentTime = DateTime.Now.AddDays(-9) }, // Comment on the image
                new Comment { PictureId = 1, CommentDescription = "Sunset is magical!", CommentTime = DateTime.Now.AddDays(-8) } // Comment on the image
            };
            // Add the comments to the database
            context.Comments.AddRange(comments);
            await context.SaveChangesAsync(); // Save changes to the database
        }

        // Seed data for comments on notes (Comments for Notes)
        if (!context.Comments.Any(k => k.NoteId == 1)) 
        {
            var noteComments = new List<Comment>
            {
                new Comment { NoteId = 1, CommentDescription = "Great start to the day!", CommentTime = DateTime.Now.AddDays(-5) }, 
                new Comment { NoteId = 1, CommentDescription = "Sounds amazing!", CommentTime = DateTime.Now.AddDays(-4) } 
            };
            
            // Add the comments for notes to the database
            context.Comments.AddRange(noteComments);
            await context.SaveChangesAsync(); // Save changes to the database
        }
    }
}
