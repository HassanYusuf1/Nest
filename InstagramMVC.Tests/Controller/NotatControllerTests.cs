using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;
using InstagramMVC.Controllers; // Adjust namespace as needed
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using InstagramMVC.Utilities;

namespace InstagramMVC.Tests.Controllers;
public class NotatControllerTests
{
    private readonly Mock<INotatRepository> _notatRepositoryMock;
    private readonly Mock<IKommentarRepository> _kommentarRepositoryMock;
    private readonly Mock<ILogger<NotatController>> _loggerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly NotatController _controller;

    public class NotatControllerTests() 
    {
        _notatRepositoryMock = new Mock<INotatRepository>();
        _kommentarRepositoryMock = new Mock<IKommentarRepository>(); // New mock for IKommentarRepository
        _loggerMock = new Mock<ILogger<NotatController>>();

        // Set up UserManager<IdentityUser> mock with default constructor parameters
        var store = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _controller = new NotatController(
            _notatRepositoryMock.Object,
            _kommentarRepositoryMock.Object, // New parameter for IKommentarRepository
            _loggerMock.Object,
            _userManagerMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Tittel", "Required");
        var newNote = new Note();

        // Act
        var result = await _controller.Create(newNote, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(newNote, viewResult.Model);
    }

    public async Task Create_SavesNoteAndCreatesDatabaseRecord_WhenValid()
    {
    // Arrange
    var userId = "user123";
    var userName = "testUser";
    var noteTitle = "Test Title";
    var noteContent = "This is a test note content.";

    // Set up a Note object to be created
    var newNote = new Note
    {
        NoteId = 10, // New note, ID will be set by the repository
        Tittel = noteTitle,
        Innhold = noteContent
    };

    // Mock UserManager to return the current user's ID and username
    var mockUser = new IdentityUser { Id = userId, UserName = userName };
    _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);

    // Mock the repository to handle the creation of the note
    _noteRepositoryMock.Setup(repo => repo.Create(It.IsAny<Note>())).ReturnsAsync(true);

    // Act
    var result = await _controller.Create(newNote);

    // Assert
    // Verify that the note was created and saved in the repository
    _noteRepositoryMock.Verify(repo => repo.Create(It.Is<Note>(n => 
        n.Tittel == noteTitle && 
        n.Innhold == noteContent &&
        n.UserId == userId)), Times.Once);

    // Check that the result is a RedirectToActionResult, redirecting to the "Index" or "Details" page
    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Notes", redirectResult.ActionName); // Change "Index" to the appropriate action name if needed
    }


}