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

    public NotatController(INotatRepository notatRepository, IKommentarRepository kommentarRepository, ILogger<NotatController> logger, UserManager<IdentityUser> userManager)
    {
        _notatRepository = notatRepository;
        _kommentarRepository = kommentarRepository;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyPage()
    {
        var currentUserName = _userManager.GetUserName(User);
        if (string.IsNullOrEmpty(currentUserName))
        {
            _logger.LogError("[NotatController] Current user is null or empty when accessing MyPage.");
            return Unauthorized();
        }

        var allNotater = await _notatRepository.GetAll();
        if (allNotater == null)
        {
            _logger.LogError("[NotatController] Could not retrieve notes for user {UserName}", currentUserName);
            return NotFound();
        }

        var userNotater = allNotater.Where(n => n.username == currentUserName).ToList();

        var notaterViewModel = new NotaterViewModel(userNotater, "MyPage");

        return View("MyPage", notaterViewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Note not found for the NoteId: {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized delete attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        return View(note);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Note for deletion not found for the NoteId: {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized delete attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        await _notatRepository.DeleteConfirmed(id);
        return RedirectToAction(nameof(Notes));
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Note note)
    {
        if (ModelState.IsValid)
        {
            note.username = _userManager.GetUserName(User);
            await _notatRepository.Create(note);
            return RedirectToAction(nameof(Notes));
        }
        _logger.LogWarning("[NotatController] Creating Note failed {@note}", note);
        return View(note);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Note not found for NoteId {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        return View(note);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(Note note)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[NotatController] Note update failed due to invalid ModelState {@note}", note);
            return View(note);
        }

        var existingNote = await _notatRepository.GetNoteById(note.NoteId);
        if (existingNote == null)
        {
            _logger.LogError("[NotatController] Note not found for update. NoteId: {NoteId}", note.NoteId);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (existingNote.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserName} for note {NoteId}", currentUserName, note.NoteId);
            return Forbid();
        }

        existingNote.Tittel = note.Tittel;
        existingNote.Innhold = note.Innhold;
        existingNote.OpprettetDato = DateTime.Now;

        await _notatRepository.Update(existingNote);
        return RedirectToAction(nameof(Notes));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Notes()
    {
        var notater = await _notatRepository.GetAll();
        if (notater == null)
        {
            _logger.LogError("[NotatController] Note List not found when running _notatRepository.GetAll()");
            return NotFound("Note List not found.");
        }
        var notaterViewModel = new NotaterViewModel(notater, "Notes");
        return View(notaterViewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NotatController] Note not found for the NoteId: {NoteId}", id);
            return NotFound("Note not found for the NoteId");
        }
        return View("Details", note);
    }
}
