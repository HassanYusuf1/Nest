using Microsoft.AspNetCore.Mvc;

using InstagramMVC.Models;
using InstagramMVC.DAL; 
using Microsoft.AspNetCore.Authorization;


namespace InstagramMVC.Controllers{

    public class BildeController : Controller
    {  
        // Må ha med for å få tilgang til database (IRepository) og loggin
        private readonly IBildeRepository _bildeRepository;
        private readonly ILogger<BildeController> _logger;

        // Konstruktør for BildeController-klassen
        public BildeController(IBildeRepository bildeRepository, ILogger<BildeController> logger){
            
            // Setter opp bildeRepository, som brukes til å jobbe med bilder i databasen
            _bildeRepository = bildeRepository;
            // Logger som brukes til å skrive meldinger som skjer i system.
            _logger = logger;

        }

        //Metode for å vise alle bilde (Grid eller tabell)
        public async Task<IActionResult> hentAlleBilder()
        {
            //Henter alle bildene fra databasen via repository
            var bilder = await _bildeRepository.GetAll();

            //Hvis bildet ikke finnes, logges en feilmelding 
            // NotFound-respons sendes til brukeren
            if  (bilder == null) 
            {
                _logger.LogError("[BildeController] Ingen bilder funnet");
                
                return NotFound("Ingen bilder funnet")
            }  



            return View(bilder);
        }

        public async Task<IActionResult> BildeDetaljer(int id)
        {
            var bilde = await _bildeRepository.GetBildeById(id);

            if (bilde == null)
            {
                _logger.LogError("[BildeController] Bildet ble ikke funnet for id {id}]");

                return NotFound("Bildet ble ikke funnet");
            }

            return View(bilde);
        }

        //Get-metode for å vise skjeamet hvor brukeren kan laste opp et nytt bilde

        [HttpGet]
        [Authorize] // Krever at brukeren er logget inn
        public async Task<ActionResult> LastOppBilde()
        {
            // Returnerer i visningen for bildeopplasting 
            return View();
        }

        
        //Post-metode for å håndtere opplasting av et nytt bilde 
        [HttpPost]
        [Authorize]// Krever at brukeren er autentisert for å få tilgang til denne metoden.
        public async Task<IActionResult> LastOppBilde(Bilde bilde)
        {
            //Sjekker om modellens tilstand er gyldig 
            // at alle nødvendige felter er riktig utfylt 

            if (ModelState.IsValid)
            {
                //Henter brukerens ID og setter den som eier. 
                bilde.BrukerId = int.Parse(User.Identity.Name);
                //Lagrer det nye bildet i databasen ved hjelp av Repo
                await _bildeRepository.Lag(hentAlleBilder);
                // Omdirigerer til brukeren til "BildeGalleri"- 
                return RedirectToAction(nameof(hentAlleBilder));
            }
            //Hvis opplastningen mislykkes, logges en advarsel, og skjemat vises på nytt.
            _logger.LogWarning("[BildeController] Opplastning av bildet mislyktes for {@bilde}", bilde);

            return View(bilde);
        }

        //GET-metode 
        [HttpGet]
        [Authorize]
        
        public async Task<IActionResult> SlettBilde (int id) 
        {
            var bilde = await _bildeRepository.GetBildeById(id);
            
            //hvis bilde ikke finnes så 
            if (bilde == null)
            {
                _logger.LogError("[BildeController] Bildet ble ikke funnet for {id}", id);
                return BadRequest("Bildet ble ikke funnet");

            }
            //returner visningen som ber om bekreftelse på sletting
            return View(bilde);
        }

        //Metode for å bekrefte sletting av bildet.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BekrefteSlettBilde (int id)
        {
            //Forsøker å slette bildet via Repository
            bool vellykket = await _bildeRepository.Slett(id);
            // hvis slettingen mislykkes, logges det en feilmelding.
            //BadRequest- repsons sendes til brukeren.
            if (!vellykket)
            {
                _logger.LogError("[BildeController] sletting av bildt mislykkes for id {id}.", id);
                return BadRequest("Sletting av bildet mislyktes");
            }

            return RedirectToAction(nameof(hentAlleBilder));
        }
        
        







    }



}