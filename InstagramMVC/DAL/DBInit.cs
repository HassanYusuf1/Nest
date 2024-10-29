using Microsoft.EntityFrameworkCore;
using InstagramMVC.Models;
using InstagramMVC.DAL;

namespace InstagramMVC.DAL;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<MediaDbContext>();

        // Aktiverer sletting og reoppretting av databasen når du ønsker
        if (true) // Sett til 'true' for å aktivere seeding
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed data for Bilder
            if (!context.Bilder.Any())
            {
                var bilder = new List<Bilde>
                {
                    new Bilde
                    {
                        Tittel = "Digg",
                        Beskrivelse = "Va.",
                        BildeUrl = "Solnedgang_JPG.jpg", // Opprett en testfil her eller last opp manuelt
                        OpprettetDato = DateTime.Now.AddDays(-10)
                    },
                    
                    
                };
                context.Bilder.AddRange(bilder);
                context.SaveChanges();
            }

            // Seed data for Kommentarer
            if (!context.Kommentarer.Any())
            {
                var kommentarer = new List<Kommentar>
                {
                    new Kommentar { BildeId = 1, KommentarBeskrivelse = "Utrolig flott bilde!", KommentarTid = DateTime.Now.AddDays(-9) },
                    new Kommentar { BildeId = 1, KommentarBeskrivelse = "Solnedgangen er magisk!", KommentarTid = DateTime.Now.AddDays(-8) },
                    new Kommentar { BildeId = 2, KommentarBeskrivelse = "For en flott fjelltur!", KommentarTid = DateTime.Now.AddDays(-4) }
                };
                context.Kommentarer.AddRange(kommentarer);
                context.SaveChanges();
            }

            // Seed data for Notater
            if (!context.Notes.Any())
            {
                var notater = new List<Note>
                {
                    new Note { Tittel = "Dagbok - Dag 1", Innhold = "Startet dagen med en god frokost og dro på tur.", OpprettetDato = DateTime.Now.AddDays(-10) },
                    new Note { Tittel = "Dagbok - Dag 2", Innhold = "Møtte noen venner for fjelltur. Fantastisk utsikt!", OpprettetDato = DateTime.Now.AddDays(-9) }
                };
                context.Notes.AddRange(notater);
                context.SaveChanges();
            }
        }
    }
}
