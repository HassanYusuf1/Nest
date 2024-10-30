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
        public IActionResult CreateComment(int bildeId)
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
        public async Task<IActionResult> CreateComment(Kommentar kommentar)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    kommentar.KommentarTid = DateTime.Now;

                    await _kommentarRepository.Create(kommentar);
                    return RedirectToAction("Details", "Bilde", new { id = kommentar.BildeId }); 
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
        public async Task<IActionResult> UpdateComment(int Id)
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
        public async Task<IActionResult> UpdateComment(Kommentar kommentar)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Ugyldig ModelState ved oppdatering av kommentar. KommentarId: {KommentarId}", kommentar.KommentarId);
                return View(kommentar);
            }
            
            try
            {
        // Hent den eksisterende kommentaren fra databasen for å få BildeId
                var eksisterendeKommentar = await _kommentarRepository.GetKommentarById(kommentar.KommentarId);
                if (eksisterendeKommentar == null)
                {
                    _logger.LogError("Fant ikke kommentar med ID {KommentarId}", kommentar.KommentarId);
                    return NotFound();
                }
                // Behold den opprinnelige BildeId-verdien for å unngå fremmednøkkelproblemer
                kommentar.BildeId = eksisterendeKommentar.BildeId;

                // Utfør oppdateringen
                await _kommentarRepository.Update(eksisterendeKommentar);

                // Omdiriger til bildedetaljsiden etter oppdateringen
                return RedirectToAction("Details", "Bilde", new { id = eksisterendeKommentar.BildeId });
                }
            catch (Exception e)
            {
                _logger.LogError("Feil oppstod under oppdatering av kommentar med ID {KommentarId}", kommentar.KommentarId);
                throw;
            }
        }

       

        [HttpGet]
        public async Task<IActionResult>  DeleteComment(int Id)
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
                return RedirectToAction("Details", "Bilde", new { id = BildeId });
            }
            catch (Exception e)
            {
                // logger feilmelding hvis sletting ikke fungerer.
                _logger.LogError("Feil ved sletting av kommentar med ID {Id}", Id);
                
                return RedirectToAction("Details", "Bilde", new { id = BildeId });
            }
        }





    }
}