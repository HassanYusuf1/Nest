using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InstagramMVC.Controllers
{
    public class BildeController : Controller
    {
        private readonly IBildeRepository _bildeRepository;
        private readonly ILogger<BildeController> _logger;

        public BildeController(IBildeRepository bildeRepository, ILogger<BildeController> logger)
        {
            _bildeRepository = bildeRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> HentAlle()
        {
            var bilder = await _bildeRepository.HentAlle();
            return Ok(bilder);
        }  

        [HttpPost("opprett")]
        public async Task<IActionResult> Opprette(Bilde nyttBilde)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Ugyldig bilde format");
            }
            bool vellykket = await _bildeRepository.Opprette(nyttBilde);
            if (vellykket)
            {
                return Ok( new {vellykket = true, message= "Bilde ble opprettet."});
            }
            else 
            {
                return BadRequest("Kunne ikke opprette bilde");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BildeId(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogWarning("Bilde med ID {ID} ikke funnet", id);
                return NotFound();
            }
            return Ok(bilde);
        }
        
        [HttpPut("oppdater/{id}")]
        public async Task<IActionResult> Oppdater(int id, Bilde bilde)
        {
            if( id!= bilde.Id || !ModelState.IsValid )
            {
                return BadRequest("Ugyldig data");
            }
            bool vellykket = await _bildeRepository.Oppdater(bilde);

            if(vellykket)
            {
                return Ok(new{vellykket= true, message = "Bilde oppdatert"});
            }
            else 
            {
                return BadRequest("Kunne ikke oppdatere Bilde");

            }

        }




        //slette bilde bilde med Id 
        [HttpDelete("slett/{id}")]
        public async Task<IActionResult> Slett(int id)
        {
            bool vellykket = await _bildeRepository.Slett(id);
            
            if(!vellykket)
            {
                return BadRequest("Kunne ikke slette bilde");
            }
            return Ok(new {vellykket = true, message = "Bilde slettet"});
        }







    }
}