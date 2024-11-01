using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InstagramMVC.Controllers
{
    public class KommentarController : Controller
    {
        private readonly IKommentarRepository _kommentarRepository;
        private readonly ILogger<KommentarController> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public KommentarController(IKommentarRepository kommentarRepository, ILogger<KommentarController> logger, UserManager<IdentityUser> userManager)
        {
            _kommentarRepository= kommentarRepository;
            _logger = logger;
            _userManager = userManager; 
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
        [Authorize]
        public async Task<IActionResult> CreateComment(Kommentar kommentar)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    kommentar.KommentarTid = DateTime.Now;

                    await _kommentarRepository.Create(kommentar);
                    return RedirectToAction("Grid", "Bilde", new { id = kommentar.BildeId }); 
                    var UserName = _userManager.GetUserName(User);
                    kommentar.UserName = UserName; 
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
        [Authorize]
        public async Task<IActionResult> UpdateComment(int Id)
        {
            var kommentar = await _kommentarRepository.GetKommentarById(Id);

            if(kommentar == null)
            {
                _logger.LogError("[KommentarController] kunne ikke finne kommentar med id {Id}", Id);
                return NotFound();
            }
            var currentUserName =  _userManager.GetUserName(User);
            if(kommentar.UserName != currentUserName)
            {
                _logger.LogWarning("Unartorized edit attempt by user {UserId} for comment {KommentarId}", currentUserName,Id);
                return Forbid();
            }
            return View(kommentar);
        }
        [HttpPost]
        [Authorize]
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

                var currentUserName = _userManager.GetUserName(User);
                if(eksisterendeKommentar.UserName != currentUserName)
                {
                    _logger.LogWarning("Unaauthorized edit attempt by use {UserName} for image {BildeId}", currentUserName);
                    return Forbid();
                }
                // Behold den opprinnelige BildeId-verdien for å unngå fremmednøkkelproblemer
                kommentar.BildeId = eksisterendeKommentar.BildeId;
                eksisterendeKommentar.KommentarBeskrivelse = kommentar.KommentarBeskrivelse; // Sett dette til det korrekte feltet som inneholder kommentarteksten
                eksisterendeKommentar.KommentarTid = DateTime.Now;
                

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
        [Authorize]
        public async Task<IActionResult>  DeleteComment(int Id)
        {
            var kommentar = await _kommentarRepository.GetKommentarById(Id);


            if(kommentar == null)
            {
                _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette det, kommentar ID : {KommentarId}", Id);
                return NotFound();
            }
            var currentUserName = _userManager.GetUserName(User);
            if (kommentar.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized delete attempt by user {UserName} for image {BildeId}", currentUserName, Id);
                return Forbid();
            }
            return View(kommentar);
        }
        [HttpPost]
        [Authorize]
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
        //FOR NOTATER
        [HttpGet]
        public IActionResult CreateCommentNote(int BildeId)
        {
            try
            {
                var kommentar = new Kommentar
                {
                    BildeId = BildeId

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
        public async Task<IActionResult> CreateCommentNote(Kommentar kommentar)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    kommentar.KommentarTid = DateTime.Now;

                    await _kommentarRepository.Create(kommentar);
                    return RedirectToAction("Details", "Notat", new { id = kommentar.BildeId }); 
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
        public async Task<IActionResult> UpdateCommentNote(int Id)
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
        public async Task<IActionResult> UpdateCommentNote(Kommentar kommentar)
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
                eksisterendeKommentar.KommentarBeskrivelse = kommentar.KommentarBeskrivelse; // Sett dette til det korrekte feltet som inneholder kommentarteksten
                eksisterendeKommentar.KommentarTid = DateTime.Now;
                

                // Utfør oppdateringen
                await _kommentarRepository.Update(eksisterendeKommentar);

                // Omdiriger til bildedetaljsiden etter oppdateringen
                return RedirectToAction("Details", "Notat", new { id = eksisterendeKommentar.BildeId });
                }
            catch (Exception e)
            {
                _logger.LogError("Feil oppstod under oppdatering av kommentar med ID {KommentarId}", kommentar.KommentarId);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult>  DeleteCommentNote(int Id)
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
        public async Task<IActionResult> DeleteConfirmedKommentarNote(int Id)
        {
            var BildeId = await _kommentarRepository.GetBildeId(Id);

            try
            {
                await _kommentarRepository.Delete(Id); // sletter kommentaren.
                // Logger en melding som viser at sletting av kommentaren var vellykket
                _logger.LogInformation("Kommentaren med Id [Kommentar Id] ble slettet", Id);
                return RedirectToAction("Details", "Notat", new { id = BildeId });
            }
            catch (Exception e)
            {
                // logger feilmelding hvis sletting ikke fungerer.
                _logger.LogError("Feil ved sletting av kommentar med ID {Id}", Id);
                
                return RedirectToAction("Details", "Notat", new { id = BildeId });
            }
        }
        
    }
}