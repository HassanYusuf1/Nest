using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;  // For Controller og IActionResult
using InstagramMVC.DAL;
using InstagramMVC.Models;
using System;

namespace InstagramMVC.Controllers
{
    public class KommentarController : Controller
    {
        private readonly IKommentarRepository _kommentarRepository;
        private readonly ILogger<KommentarController> _logger;

        public KommentarController(IKommentarRepository kommentarRepository, ILogger<KommentarController> logger)
        {
            _kommentarRepository= kommentarRepository;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult CreateKommentar(int bildeId)
        {
            try
            {
                var kommentar = new Kommentar
                {
                    BildeId= bildeId

                };
                return View(kommentar);


            }
            //Nye kommentarer return til view
           
            catch(Exception e)
            {
                _logger.LogError("Oppretting av ny kommentar feilet",e);
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateKommentar(Kommentar kommentar)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    kommentar.KommentarTid = DateTime.Now;

                    await _kommentarRepository.Create(kommentar);
                    return RedirectToAction("BildeInfo", "Bilde", new { id =kommentar.BildeId});
                }
                _logger.LogWarning("[KommentarController] Opprettning av ny kommentar feilet, Modelstat funker ikke");
                return View(kommentar);
            }

            catch(Exception e)
            {
                _logger.LogError("Feil skjedde under oppretting av kommentar", e);
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdateKommentar(int Id)
        {
            var kommentar = await _kommentarRepository.GetKommentarById(Id);

            if(kommentar == null)
            {
                _logger.LogError("[KommentarController] kunne ikke finne kommentar med id {Id}", Id);
                return NotFound();
            }
            return View(kommentar);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateKommentar(Kommentar kommentar)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Ugylding Modeltilstand når man forsøker å oppdatere kommentar. KommentarId: {KommentarId}",kommentar.KommentarId);
                return View(kommentar);
            }

            try
            {
                await _kommentarRepository.Update(kommentar);

            }
            catch(Exception e)
            {
                _logger.LogError("Feil oppstod under opppdatering av kommentar med ID {KommentarId}", kommentar.KommentarId);
                throw; 
            }

              return RedirectToAction("BildeDetaljer", new { Id = kommentar.BildeId });
            
        }
        [HttpGet]
        public async Task<IActionResult>  DeleteKommentar(int Id)
        {
            var kommentar = await _kommentarRepository.GetKommentarById(Id);


            if(kommentar == null)
            {
                _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette det, kommentar ID : {KommentarId}", Id);
                return NotFound();
            }
            
            return View(kommentar);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedKommentar(int Id)
        {
            var BildeId = await _kommentarRepository.GetBildeId(Id);

            try
            {
                await _kommentarRepository.Delete(Id); // sletter kommentaren.
                // Logger en melding som viser at sletting av kommentaren var vellykket
                _logger.LogInformation("Kommentaren med Id [Kommentar Id] ble slettet", Id);
                return RedirectToAction("BildeDetaljer", "Bilde", new {Id = BildeId});
            }
            catch (Exception e)
            {
                // logger feilmelding hvis sletting ikke fungerer.
                _logger.LogError("Feil ved sletting av kommentar med ID {Id}", Id);
                
                return RedirectToAction("BildeDetaljer", "Bilde" , new { Id = BildeId});
            }
        }





    }
}