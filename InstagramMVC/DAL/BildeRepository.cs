using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using Microsoft.Extensions.Logging;


namespace InstagramMVC.DAL
{
    public class BildeRepository : IBildeRepository
    {
        private readonly MediaDbContext _context;
        private readonly ILogger<BildeRepository> _logger;

        public BildeRepository(MediaDbContext context, ILogger<BildeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Bilde>?> HentAlle()
        {
            try
            {
                return await _context.Bilder.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[BildeRepository] HentAlle feilet ved henting av alle bilder. Feilmelding: {e}", e.Message);
                return null;
            }
        }


    }
}