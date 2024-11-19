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

    TempData["Source"] = source; // Store source in TempData for later use in view
    return View(comment);
}

[HttpPost]
[Authorize]
public async Task<IActionResult> EditComment(int Id, Comment updatedComment, string source)
{
    if (Id != updatedComment.CommentId || !ModelState.IsValid)
    {
        TempData["Source"] = source; // Preserve source value in case of validation error
        return View(updatedComment);
    }

    var existingComment = await _CommentRepository.GetCommentById(Id);
    if (existingComment == null)
    {
        _logger.LogError("Could not find comment ID {CommentId}", updatedComment.CommentId);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (existingComment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, updatedComment.CommentId);
        return Forbid();
    }

    // Update the comment content and timestamp
    existingComment.CommentDescription = updatedComment.CommentDescription;
    existingComment.CommentTime = DateTime.Now;

    bool success = await _CommentRepository.Edit(existingComment);
    if (success)
    {
        // Redirect to the correct page based on the Source parameter
        return RedirectToAction(source == "MyPage" ? "MyPage" : "Grid", "Picture");
    }
    else
    {
        _logger.LogWarning("[CommentController] Could not update the comment.");
        TempData["Source"] = source; // Preserve source value in case of update failure
        return View(updatedComment);
    }
}




[HttpGet]
[Authorize]
public async Task<IActionResult> DeleteComment(int id, string source = "Grid")
{
    var comment = await _CommentRepository.GetCommentById(id);

    if (comment == null)
    {
        _logger.LogWarning("Comment not found when trying to delete, comment ID: {CommentId}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {CommentId}", currentUserName, id);
        return Forbid();
    }

    // Store the source in TempData for later use in the POST method.
    TempData["Source"] = source;

    return View(comment);
}


[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmedComment(int id, string source)
{
    var comment = await _CommentRepository.GetCommentById(id);
    if (comment == null)
    {
        _logger.LogWarning("[CommentController] Comment with Id {CommentId} not found", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for comment {CommentId}", currentUserName, id);
        return Forbid();
    }

    bool success = await _CommentRepository.Delete(id);

    if (!success)
    {
        _logger.LogError("[CommentController] Comment with Id {CommentId} was not deleted successfully", id);
        TempData["Source"] = source; // Preserve source value in case of deletion failure
        return BadRequest("Comment not deleted");
    }

    // Redirect to the correct page based on the Source parameter
    return RedirectToAction(source == "MyPage" ? "MyPage" : "Grid", "Picture");
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
public async Task<IActionResult> EditCommentNote(int id, string source = "Notes")
{
    var comment = await _CommentRepository.GetCommentById(id);

    if (comment == null)
    {
        _logger.LogError("[CommentController] Could not find comment with id {Id}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (comment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, id);
        return Forbid();
    }

    TempData["Source"] = source; // Store source in TempData for later use in view
    return View(comment);
}


[HttpPost]
[Authorize]
public async Task<IActionResult> EditCommentNote(int id, Comment updatedComment, string source)
{
    if (id != updatedComment.CommentId || !ModelState.IsValid)
    {
        TempData["Source"] = source; // Preserve source value in case of validation error
        return View(updatedComment);
    }

    var existingComment = await _CommentRepository.GetCommentById(id);
    if (existingComment == null)
    {
        _logger.LogError("Could not find comment ID {CommentId}", updatedComment.CommentId);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (existingComment.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized edit attempt by user {UserName} for comment {CommentId}", currentUserName, updatedComment.CommentId);
        return Forbid();
    }

    // Update the comment content and timestamp
    existingComment.CommentDescription = updatedComment.CommentDescription;
    existingComment.CommentTime = DateTime.Now;

    bool success = await _CommentRepository.Edit(existingComment);
    if (success)
    {
        // Redirect to the correct page based on the Source parameter
        return RedirectToAction(source == "Notes" ? "Notes" : "MyPage", "Note", new { id = existingComment.NoteId });
    }
    else
    {
        _logger.LogWarning("[CommentController] Could not update the comment.");
        TempData["Source"] = source; // Preserve source value in case of update failure
        return View(updatedComment);
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