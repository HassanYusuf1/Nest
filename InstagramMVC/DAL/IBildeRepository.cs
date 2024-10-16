
using InstagramMVC.Models;

namespace InstagramMVC.DAL{
public interface IBildeRepository{

    //Henter alle bilder fra databasen
    Task<IEnumerable<Bilde>> GetAll();
    
    Task<Bilde?> GetBildeById(int id);
    Task <bool>  Create(Bilde bilde);

    Task <bool>  Slett(int id);
}
}
