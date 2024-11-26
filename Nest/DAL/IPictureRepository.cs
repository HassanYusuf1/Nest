using System.Collections.Generic;
using System.Threading.Tasks;
using Nest.Models;

namespace Nest.DAL
{
    public interface IPictureRepository
    {
        //Henter Alle pictures fra databasen
        Task<IEnumerable<Picture>?> GetAll();

        //Opprette et nytt picture
        Task<bool> Create(Picture picture);

        // Hente Picture basert på ID
        Task<Picture?> PictureId(int id);

        // Updateer en eksisterende picture   
        Task<bool> Edit(Picture picture);

        // Sletter et picture basert på ID.
        Task<bool> Delete(int id);



    }


}