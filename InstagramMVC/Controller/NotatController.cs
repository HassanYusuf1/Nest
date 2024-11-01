using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InstagramMVC.Controllers;

public class NotatController : Controller
{
    private readonly ILogger<NotatController> _logger;
    private readonly IKommentarRepository _kommentarRepository;
    private readonly INotatRepository _notatRepository;
    private readonly UserManager<IdentityUser> _userManager;


    public NotatController(INotatRepository notatRepository, IKommentarRepository kommentarRepository,ILogger<NotatController> logger, UserManager<IdentityUser> userManager)
    {
        _notatRepository = notatRepository;
        _kommentarRepository = kommentarRepository;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Notes()
    {
        var notater = await _notatRepository.GetAll();
        if (notater == null)
        {
            _logger.LogError("[NotatController] Note List not found when running _notatRepository.GetAll()");
            return NotFound("Note List not found.");
        }
        var notaterViewModel = new NotaterViewModel(notater, "Notat");
        return View(notaterViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
            _logger.LogError("[NotatController] Note not found for the NoteId: {NoteId:}", id);
            return NotFound("Note not found for the NoteId");
        return View("Details", note);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Note not found for the NoteId: {NoteId:}", id);
            return BadRequest("Could not delete note.");
        }
        return View(note);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var notat = await _notatRepository.GetNoteById(id);
        if (notat == null)
        {
            _logger.LogError("[NotatController] Note for deletion not found for the NoteId: {NoteId:}", id);
            return BadRequest("Could not delete Note");
        }
        await _notatRepository.DeleteConfirmed(id);
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Note note)
    {
        if (ModelState.IsValid)
        {
            await _notatRepository.Create(note);
            
            return RedirectToAction(nameof(Index));
        }
        _logger.LogWarning("[NotatController] Creating Note failed {@note}", note);
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Creating Note failed {@note}", note);
            return BadRequest("Note not found for NoteId");
        }
        return View(note);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(Note note)
    {
        if (ModelState.IsValid)
        {
            await _notatRepository.Update(note);
            
            return RedirectToAction(nameof(Index));
        }
        _logger.LogWarning("[NotatController] Note update failed {@note}", note);
        return View(note);
    }
}
