using System.Collections.Generic;
using System.Threading.Tasks;
using InstagramMVC.Models;

namespace InstagramMVC.DAL
{
    public interface IBildeRepository 
    {
        //Henter Alle bilder fra databasen
        Task<IEnumerable<Bilde>> HentAlle();

         

    }


}