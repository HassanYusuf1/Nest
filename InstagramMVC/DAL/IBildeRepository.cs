
using InstagramMVC.Models;

public interface IBildeRepository{

    //Henter alle bilder fra databasen
    Task<IEnumerable<Bilde>> GetAll();
    
    Task<Bilde?> GetBildeById(int id);
    Task <bool>  Lag (Bilde bilde);

    Task <bool>  Slett(int id);
}

