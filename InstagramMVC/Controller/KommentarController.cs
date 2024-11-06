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
                _logger.LogError(e, "Oppretting av ny kommentar feilet");
                throw;
            }
        }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment(Kommentar kommentar)
    {
        try
        {
            if (ModelState.IsValid)
            {
                kommentar.KommentarTid = DateTime.Now;
                kommentar.UserName = _userManager.GetUserName(User);
                await _kommentarRepository.Create(kommentar);

                return RedirectToAction("Grid", "Bilde", new { id = kommentar.BildeId });
            }

            _logger.LogWarning("[KommentarController] Opprettning av ny kommentar feilet, ModelState funker ikke");
            return View(kommentar);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Feil skjedde under oppretting av kommentar");
            throw;
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> UpdateComment(int Id)
    {
        var kommentar = await _kommentarRepository.GetKommentarById(Id);

        if (kommentar == null)
        {
            _logger.LogError("[KommentarController] kunne ikke finne kommentar med id {Id}", Id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (kommentar.UserName != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {KommentarId}", currentUserName, Id);
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
            var eksisterendeKommentar = await _kommentarRepository.GetKommentarById(kommentar.KommentarId);
            if (eksisterendeKommentar == null)
            {
                _logger.LogError("Fant ikke kommentar med ID {KommentarId}", kommentar.KommentarId);
                return NotFound();
            }

            var currentUserName = _userManager.GetUserName(User);
            if (eksisterendeKommentar.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {KommentarId}", currentUserName, kommentar.KommentarId);
                return Forbid();
            }

            eksisterendeKommentar.KommentarBeskrivelse = kommentar.KommentarBeskrivelse;
            eksisterendeKommentar.KommentarTid = DateTime.Now;

        await _kommentarRepository.Update(eksisterendeKommentar);

        return RedirectToAction("Details", "Bilde", new { id = eksisterendeKommentar.BildeId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Feil oppstod under oppdatering av kommentar med ID {KommentarId}", kommentar.KommentarId);
        throw;
    }
}

[HttpGet]
[Authorize]
public async Task<IActionResult> DeleteComment(int Id)
{
    var kommentar = await _kommentarRepository.GetKommentarById(Id);

    if (kommentar == null)
    {
        _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette det, kommentar ID : {KommentarId}", Id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (kommentar.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {KommentarId}", currentUserName, Id);
        return Forbid();
    }

    return View(kommentar);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmedKommentar(int Id)
{
    var kommentar = await _kommentarRepository.GetKommentarById(Id);
    if (kommentar == null)
    {
        _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette det, kommentar ID : {KommentarId}", Id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (kommentar.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {KommentarId}", currentUserName, Id);
        return Forbid();
    }

    var BildeId = kommentar.BildeId;

    try
    {
        await _kommentarRepository.Delete(Id);
        _logger.LogInformation("Kommentaren med Id {KommentarId} ble slettet", Id);
        return RedirectToAction("Details", "Bilde", new { id = BildeId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Feil ved sletting av kommentar med ID {Id}", Id);
        return RedirectToAction("Details", "Bilde", new { id = BildeId });
    }
}

        //FOR NOTATER
       [HttpGet]
[Authorize]
public IActionResult CreateCommentNote(int noteId)
{
    try
    {
        var kommentar = new Kommentar
        {
            NoteId = noteId
        };
        return View(kommentar);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Oppretting av ny kommentar feilet");
        throw;
    }
}

[HttpPost]
[Authorize]
public async Task<IActionResult> CreateCommentNote(Kommentar kommentar)
{
    try
    {
        if (ModelState.IsValid)
        {
            kommentar.KommentarTid = DateTime.Now;
            kommentar.UserName = _userManager.GetUserName(User);

            await _kommentarRepository.Create(kommentar);
            return RedirectToAction("Notes", "Notat");
        }

        _logger.LogWarning("[KommentarController] Opprettning av ny kommentar for notat feilet, ModelState er ugyldig");
        return View(kommentar);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Feil skjedde under oppretting av kommentar");
        throw;
    }
}

[HttpGet]
[Authorize]
public async Task<IActionResult> UpdateCommentNote(int id)
{
    var kommentar = await _kommentarRepository.GetKommentarById(id);

    if (kommentar == null)
    {
        _logger.LogError("[KommentarController] Kunne ikke finne kommentar med id {Id}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (kommentar.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {KommentarId}", currentUserName, id);
        return Forbid();
    }

    return View(kommentar);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> UpdateCommentNote(Kommentar kommentar)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Ugyldig ModelState ved oppdatering av kommentar. KommentarId: {KommentarId}", kommentar.KommentarId);
        return View(kommentar);
    }

    try
    {
        var eksisterendeKommentar = await _kommentarRepository.GetKommentarById(kommentar.KommentarId);
        if (eksisterendeKommentar == null)
        {
            _logger.LogError("Fant ikke kommentar med ID {KommentarId}", kommentar.KommentarId);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (eksisterendeKommentar.UserName != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {KommentarId}", currentUserName, kommentar.KommentarId);
            return Forbid();
        }

        eksisterendeKommentar.KommentarBeskrivelse = kommentar.KommentarBeskrivelse;
        eksisterendeKommentar.KommentarTid = DateTime.Now;

        await _kommentarRepository.Update(eksisterendeKommentar);

        return RedirectToAction("Details", "Notat", new { id = eksisterendeKommentar.NoteId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Feil oppstod under oppdatering av kommentar med ID {KommentarId}", kommentar.KommentarId);
        throw;
    }
}

[HttpGet]
[Authorize]
public async Task<IActionResult> DeleteCommentNote(int id)
{
    var kommentar = await _kommentarRepository.GetKommentarById(id);

    if (kommentar == null)
    {
        _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette den, kommentar ID : {KommentarId}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (kommentar.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {KommentarId}", currentUserName, id);
        return Forbid();
    }

    return View(kommentar);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmedKommentarNote(int id)
{
    var kommentar = await _kommentarRepository.GetKommentarById(id);
    if (kommentar == null)
    {
        _logger.LogWarning("Kommentar ble ikke funnet når man prøver å slette den, kommentar ID : {KommentarId}", id);
        return NotFound();
    }

    var noteId = kommentar.NoteId;

    try
    {
        await _kommentarRepository.Delete(id);
        _logger.LogInformation("Kommentaren med ID {KommentarId} ble slettet", id);
        return RedirectToAction("Details", "Notat", new { id = noteId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Feil ved sletting av kommentar med ID {Id}", id);
        return RedirectToAction("Details", "Notat", new { id = noteId });
    }
}
        
    }
}