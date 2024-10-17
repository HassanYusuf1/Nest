using System.Collections.Generic;
using System.Threading.Tasks;
using InstagramMVC.Models;

namespace InstagramMVC.DAL
{
    public interface IBildeRepository 
    {
        //Henter Alle bilder fra databasen
        Task<IEnumerable<Bilde>?> HentAlle();

        //Opprette et nytt bilde
        Task<bool> Opprette(Bilde bilde);

        // Hente Bilde basert på ID
        //Task<Bilde?> BildeId(int id);

         

    }


}