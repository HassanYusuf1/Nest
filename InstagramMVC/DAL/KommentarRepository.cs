
using InstagramMVC.Models;
using Microsoft.EntityFrameworkCore;
 
namespace InstagramMVC.DAL
{
    public class KommentarRepository
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
    }
}