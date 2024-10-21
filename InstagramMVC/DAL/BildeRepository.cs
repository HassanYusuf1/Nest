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

        public async Task<IEnumerable<Bilde>?> GetAll()
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

        public async Task<bool> Opprette(Bilde bilde)
        {
            try
            {
                await _context.Bilder.AddAsync(bilde);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError("[BildeRespository] Feil ved opprettelse av {@bilde}, FeilMelding: {e} ", bilde , e.Message);
                return false;
            }
        }

        public async Task<Bilde?> BildeId(int id)
        {
            try
            {
                return await _context.Bilder.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[BildeRepository] Feilet ved henting av bilde med ID {id} (FindAsync), Feilmelding: {e}", id, e.Message);
                return null;
            }
        }

        public async  Task<bool> Oppdater(Bilde bilde)
        {
           try{
            _context.Bilder.Update(bilde);
            await _context.SaveChangesAsync();
            return true;
           }
           catch(Exception e)
           {
            _logger.LogError("[BildeRepository] Oppdatering av bilde med ID {ID} feilet, melding {e}", bilde.Id,e.Message);
            return false;
           }
        }

        public async Task<bool> Slett(int id)
        {
            try
            {
                var bilde = await _context.Bilder.FindAsync(id);
                if (bilde == null)
                {
                    _logger.LogError("[BildeRepository] Sletting av bilde mislyktes. Bilde med ID {id} ble ikke funnet",id);
                    return false;
                }
                _context.Bilder.Remove(bilde);
                await _context.SaveChangesAsync();
                return true;

            }
            catch(Exception e)
            {
                _logger.LogError("[BildeRepository] Sletting av bilde med ID {id} feilet, melding {e}", id, e.Message);
                return false;

            }
        }


    }
}