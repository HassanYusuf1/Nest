
using InstagramMVC.Models;
using Microsoft.EntityFrameworkCore;
 
namespace InstagramMVC.DAL
{
    public class KommentarRepository : IKommentarRepository
    {
        private readonly MediaDbContext _context;
        private readonly ILogger<KommentarRepository> _logger;

        public KommentarRepository( MediaDbContext context, ILogger<KommentarRepository> logger )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Kommentar>> GetAll()
        {
            try
            {
                return await _context.Kommentarer.ToListAsync();

            }
            catch(Exception e)
            {
                _logger.LogError("[KommentarRepository] Kommentar ToListAsync Feilet, Når GetAll() ble brukt, error Melding", e.Message);
                return null;
            }
        }


        public async Task<Kommentar?> GetKommentarById(int id)
        {
            try
            {
                return await _context.Kommentarer.FindAsync(id);

            }
            catch(Exception e)
            {
                _logger.LogError("[KommentarRepository] Feilet når man bruker GetKommentarById for KommentarId {KommentarId:0000}, error melding: {e}", e.Message);
                return null;

            }
        }

        public async Task<int?> GetBildeId(int id)
        {
            var kommentar = await _context.Kommentarer.FindAsync(id);
            return kommentar?.BildeId;
        }

        public async Task Create(Kommentar kommentar)
        {
            try
            {
                _context.Kommentarer.Add(kommentar);
                await _context.SaveChangesAsync();

            }
            catch(Exception e)
            {
                _logger.LogError("[KommentarRepository] feil ved oppretning av kommentar med id {@kommentar}, error melding", kommentar, e.Message);
            }
        }

        public async Task Update(Kommentar kommentar)
        {
            _context.Kommentarer.Update(kommentar);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var kommentar = await _context.Kommentarer.FindAsync(id);

                if(kommentar == null)
                {
                    return false;
                }
                _context.Kommentarer.Remove(kommentar);
                await _context.SaveChangesAsync();
                return true;
            }

            
            catch(Exception e)
            {
                _logger.LogError("[KommentarRepository] feilet ved sletting av kommentar med ID {KommentarId:0000}, error Melding:", id, e.Message);
                return false;
            }
                
        }

    }
}