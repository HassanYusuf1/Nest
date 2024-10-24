using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;  // For Controller og IActionResult
using InstagramMVC.DAL;
using InstagramMVC.Models;

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



    }
}