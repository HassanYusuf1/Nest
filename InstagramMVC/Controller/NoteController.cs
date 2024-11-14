using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InstagramMVC.Controllers;

public class NoteController : Controller
{
    private readonly ILogger<NoteController> _logger;
    private readonly ICommentRepository _commentRepository;
    private readonly INoteRepository _noteRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public NoteController(INoteRepository noteRepository, ICommentRepository commentRepository, ILogger<NoteController> logger, UserManager<IdentityUser> userManager)
    {
        _noteRepository = noteRepository;
        _commentRepository = commentRepository;
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
        _logger.LogError("[NoteController] Current user is null or empty when accessing MyPage.");
        return Unauthorized();
    }

    var allNotes = await _noteRepository.GetAll();
    if (allNotes == null)
    {
        _logger.LogError("[NoteController] Could not retrieve notes for user {UserName}", currentUserName);
        return NotFound();
    }

    var userNotes = allNotes.Where(n => n.username == currentUserName).ToList();
    var notesViewModel = new NotesViewModel(userNotes, "MyPage");
    
    ViewData["IsMyPage"] = true; // Set the source for MyPage

    return View("MyPage", notesViewModel);
}


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id, string source = "Notes")
    {
        var note = await _noteRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NoteController] Note not found for the NoteId: {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized delete attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        TempData["Source"] = source; // Store source in TempData
        return View(note);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id, string source)
    {
        var note = await _noteRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NoteController] Note for deletion not found for the NoteId: {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized delete attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        await _noteRepository.DeleteConfirmed(id);

        // Redirect to the correct page based on the Source parameter
        return RedirectToAction(source == "MyPage" ? "MyPage" : "Notes");
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
            await _noteRepository.Create(note);
            return RedirectToAction(nameof(MyPage));
        }
        _logger.LogWarning("[NoteController] Creating Note failed {@note}", note);
        return View(note);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int id, string source = "Notes")
    {
        var note = await _noteRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NoteController] Note not found for NoteId {NoteId}", id);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (note.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for note {NoteId}", currentUserName, id);
            return Forbid();
        }

        TempData["Source"] = source; // Store source in TempData for use in redirection
        return View(note);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(Note note, string source)
    {
        if (!ModelState.IsValid)
        {
            TempData["Source"] = source; // Preserve source value in case of validation error
            _logger.LogWarning("[NoteController] Note update failed due to invalid ModelState {@note}", note);
            return View(note);
        }

        var existingNote = await _noteRepository.GetNoteById(note.NoteId);
        if (existingNote == null)
        {
            _logger.LogError("[NoteController] Note not found for update. NoteId: {NoteId}", note.NoteId);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (existingNote.username != currentUserName)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserName} for note {NoteId}", currentUserName, note.NoteId);
            return Forbid();
        }

        existingNote.Title = note.Title;
        existingNote.Content = note.Content;
        existingNote.UploadDate = DateTime.Now;

        await _noteRepository.Edit(existingNote);

        // Redirect to the correct page based on the Source parameter
        return RedirectToAction(source == "MyPage" ? "MyPage" : "Notes");
    }


   [HttpGet]
[Authorize]
public async Task<IActionResult> Notes()
{
    var notes = await _noteRepository.GetAll();
    var notesViewModel = new NotesViewModel(notes, "Notes");
    if (notes == null)
    {
        _logger.LogError("[NoteController] Note List not found when running _noteRepository.GetAll()");
        return NotFound("Note List not found.");
    }
    
    
    ViewData["IsMyPage"] = false; // Set the source for general feed
    
    return View(notesViewModel);
}

    [HttpGet]
    public async Task<IActionResult> Details(int id, string source = "Notes")
    {
        var note = await _noteRepository.GetNoteById(id);
        if (note == null)
        {
            _logger.LogError("[NoteController] Note not found for the NoteId: {NoteId}", id);
            return NotFound("Note not found for the NoteId");
        }

        ViewBag.Source = source; // Lagre source i ViewBag for bruk i visningen
        return View("Details", note);
    }

}
