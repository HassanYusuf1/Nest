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
    public class CommentController : Controller
    {
        private readonly ICommentRepository _CommentRepository;
        private readonly ILogger<CommentController> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public CommentController(ICommentRepository CommentRepository, ILogger<CommentController> logger, UserManager<IdentityUser> userManager)
        {
            _CommentRepository= CommentRepository;
            _logger = logger;
            _userManager = userManager; 
        }
        [HttpGet]
        public IActionResult CreateComment(int pictureId)
        {
            try
            {
                var Comment = new Comment
                {
                    PictureId= pictureId

                };
                return View(Comment);


            }
            //Nye Commenter return til view
           
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to upload comment");
                throw;
            }
        }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment(Comment Comment)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Comment.CommentTime = DateTime.Now;
                Comment.UserName = _userManager.GetUserName(User);
                await _CommentRepository.Create(Comment);

                return RedirectToAction("Grid", "Picture", new { id = Comment.PictureId });
            }

            _logger.LogWarning("[CommentController] Failed to upload comment, ModelState not working");
            return View(Comment);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during comment upload");
            throw;
        }
    }

    [HttpGet]
[Authorize]
public async Task<IActionResult> EditComment(int Id, string source = "Grid")
{
    var comment = await _CommentRepository.GetCommentById(Id);

    if (comment == null)
    {
        _logger.LogError("[CommentController] Could not find comment with id {Id}", Id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, Id);
        return Forbid();
    }

    TempData["Source"] = source; // Store source for later use in view
    return View(comment);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> EditComment(Comment comment, string source)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Error comment update, invalid ModelState. CommentId: {CommentId}", comment.CommentId);
        TempData["Source"] = source; // Preserve source value in case of validation error
        return View(comment);
    }

    try
    {
        var existingComment = await _CommentRepository.GetCommentById(comment.CommentId);
        if (existingComment == null)
        {
            _logger.LogError("Could not find comment ID {CommentId}", comment.CommentId);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (existingComment.UserName != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, comment.CommentId);
            return Forbid();
        }

        // Update the comment content and timestamp
        existingComment.CommentDescription = comment.CommentDescription;
        existingComment.CommentTime = DateTime.Now;

        await _CommentRepository.Edit(existingComment);

        // Redirect to the correct page based on the source parameter
        return RedirectToAction(source == "MyPage" ? "MyPage" : "Grid");
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error during comment update, ID {CommentId}", comment.CommentId);
        throw;
    }
}



[HttpGet]
[Authorize]
public async Task<IActionResult> DeleteComment(int Id)
{
    var Comment = await _CommentRepository.GetCommentById(Id);

    if (Comment == null)
    {
        _logger.LogWarning("Comment not found when trying to delete, comment ID : {CommentId}", Id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (Comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {CommentId}", currentUserName, Id);
        return Forbid();
    }

    return View(Comment);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmedComment(int Id)
{
    var Comment = await _CommentRepository.GetCommentById(Id);
    if (Comment == null)
    {
        _logger.LogWarning("Comment not found when trying to delete, comment ID : {CommentId}", Id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (Comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {CommentId}", currentUserName, Id);
        return Forbid();
    }

    var PictureId = Comment.PictureId;

    try
    {
        await _CommentRepository.Delete(Id);
        _logger.LogInformation("Commenten with Id {CommentId} ble slettet", Id);
        return RedirectToAction("Details", "Picture", new { id = PictureId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error delete comment with ID {Id}", Id);
        return RedirectToAction("Details", "Picture", new { id = PictureId });
    }
}

        //FOR NOTATER
    [HttpGet]
    [Authorize]
    public IActionResult CreateCommentNote(int noteId)
    {
        try
        {
            var Comment = new Comment
            {
                NoteId = noteId
            };
            return View(Comment);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error create new comment");
            throw;
        }
    }

[HttpPost]
[Authorize]
public async Task<IActionResult> CreateCommentNote(Comment Comment)
{
    try
    {
        if (ModelState.IsValid)
        {
            Comment.PictureId = null;
            Comment.CommentTime = DateTime.Now;
            Comment.UserName = _userManager.GetUserName(User);

            await _CommentRepository.Create(Comment);
            return RedirectToAction("Notes", "Note", new { id = Comment.NoteId });
            
        }
                                                
        _logger.LogWarning("[CommentController] Error new note upload, ModelState invalid");
        return View(Comment);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error comment upload");
        throw;
    }
}

[HttpGet]
[Authorize]
public async Task<IActionResult> EditCommentNote(int id)
{
    var Comment = await _CommentRepository.GetCommentById(id);

    if (Comment == null)
    {
        _logger.LogError("[CommentController] Could not fint comment with id {Id}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (Comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, id);
        return Forbid();
    }

    return View(Comment);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> EditCommentNote(Comment Comment)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("INvalid ModelState by comment update. CommentId: {CommentId}", Comment.CommentId);
        return View(Comment);
    }

    try
    {
        var existingComment = await _CommentRepository.GetCommentById(Comment.CommentId);
        if (existingComment == null)
        {
            _logger.LogError("Could not find comment with ID {CommentId}", Comment.CommentId);
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if (existingComment.UserName != currentUserName)
        {
            _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, Comment.CommentId);
            return Forbid();
        }

        existingComment.CommentDescription = Comment.CommentDescription;
        existingComment.CommentTime = DateTime.Now;

        await _CommentRepository.Edit(existingComment);

        return RedirectToAction("Notes", "Note", new { id = existingComment.NoteId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error comment update with ID {CommentId}", Comment.CommentId);
        throw;
    }
}

[HttpGet]
[Authorize]
public async Task<IActionResult> DeleteCommentNote(int id)
{
    var Comment = await _CommentRepository.GetCommentById(id);

    if (Comment == null)
    {
        _logger.LogWarning("Comment not found when trying to delete, comment ID : {CommentId}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (Comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {CommentId}", currentUserName, id);
        return Forbid();
    }

    return View(Comment);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmedCommentNote(int id)
{
    var Comment = await _CommentRepository.GetCommentById(id);
    if (Comment == null)
    {
        _logger.LogWarning("Comment not found when trying to delete, comment ID : {CommentId}", id);
        return NotFound();
    }

    var noteId = Comment.NoteId;

    try
    {
        await _CommentRepository.Delete(id);
        _logger.LogInformation("Comment with ID {CommentId} ble slettet", id);
        return RedirectToAction("Notes", "Note", new { id = noteId });
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error delete comment with ID {Id}", id);
        return RedirectToAction("Notes", "Note", new { id = noteId });
    }
}
        
    }
}