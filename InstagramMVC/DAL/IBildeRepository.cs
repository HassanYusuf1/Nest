using System.Collections.Generic;
using System.Threading.Tasks;
using InstagramMVC.Models;

namespace InstagramMVC.DAL
{
    public interface IBildeRepository 
    {
        //Henter Alle bilder fra databasen
        Task<IEnumerable<Bilde>?> GetAll();

        //Opprette et nytt bilde
        Task<bool> Opprette(Bilde bilde);

        // Hente Bilde basert på ID
        Task<Bilde?> BildeId(int id);

        // Oppdaterer en eksisterende bilde   
        Task<bool> Oppdater(Bilde bilde);

        // Sletter et bilde basert på ID.
        Task<bool> Delete(int id);

         

    }


}