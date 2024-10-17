using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // for å samhandle med data ved hjelp av entity Framework
using Microsoft.Extensions.Logging;
using InstagramMVC.Models;


namespace InstagramMVC.DAL
{
    public class BildeRepository : //IBildeRepository
    {
        private readonly MediaDbContext _db;
        private readonly ILogger<BildeRepository> _logger;

        //konstruktør som initialiser DBcontext og logger
        public BildeRepository (MediaDbContext db, ILogger<BildeRepository> logger)
        {
            _db = db;
            _logger= logger;

        }

        public async Task<IEnumerable<Bilde>> GetAll()
        {
            try
            {
                //henter alle bilder som en liste
                return await _db.Bilder.ToListAsync();
            }
            catch(Exception e){
                _logger.LogError("[BildeRepository] GetAll() feilet, Melding: {e}", e.Message);
                
                return null; 
            }
        }
    }
}
