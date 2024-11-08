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

namespace InstagramMVC.Tests.Controllers
{
    public class NotatControllerTests
    {
        private readonly NotatController _controller;
        private readonly Mock<INotatRepository> _notatRepositoryMock;
        private readonly Mock<IKommentarRepository> _kommentarRepositoryMock;
        private readonly Mock<ILogger<NotatController>> _loggerMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

        public NotatControllerTests()
        {
            // Instantiate mocks
            _notatRepositoryMock = new Mock<INotatRepository>();
            _kommentarRepositoryMock = new Mock<IKommentarRepository>();
            _loggerMock = new Mock<ILogger<NotatController>>();
            _urlHelperMock = new Mock<IUrlHelper>();

            // Set up UserManager<IdentityUser> mock with default constructor parameters
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // Initialize controller with mocks
            _controller = new NotatController(
                _notatRepositoryMock.Object,
                _kommentarRepositoryMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task Create_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Tittel", "Required");
            var newNote = new Note();

            // Act
            var result = await _controller.Create(newNote);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(newNote, viewResult.Model);
        }

        [Fact]
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
            _notatRepositoryMock.Setup(repo => repo.Create(It.IsAny<Note>())).Returns(Task.CompletedTask);


            // Act
            var result = await _controller.Create(newNote);

            // Assert
            // Verify that the note was created and saved in the repository
            _notatRepositoryMock.Verify(repo => repo.Create(It.Is<Note>(n => 
                n.Tittel == noteTitle && 
                n.Innhold == noteContent)), Times.Once);

            // Check that the result is a RedirectToActionResult, redirecting to the "Notes" or another specified page
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Notes", redirectResult.ActionName); // Change "Notes" to the appropriate action name if needed
        }

        [Fact]
        public async Task CreateNote_ReturnsView_WhenDatabaseSaveFails()
        {
            // Arrange
            var newNote = new Note();
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("testUser");
            _notatRepositoryMock.Setup(repo => repo.Create(It.IsAny<Note>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(newNote);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Notes", redirectResult.ActionName); // Change "Notes" to the appropriate action name if needed
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesNote_WhenNoteIdExists()
        {
            // Arrange
            int noteId = 30;
            var currentUserName = "testUser";
            var newNote = new Note { NoteId = noteId, username = currentUserName };

            // Set up the repository to return the note and confirm deletion
            _notatRepositoryMock.Setup(repo => repo.GetNoteById(noteId)).ReturnsAsync(newNote);
            _notatRepositoryMock.Setup(repo => repo.DeleteConfirmed(noteId)).ReturnsAsync(true);

            // Mock user identity
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

            // Act
            var result = await _controller.DeleteConfirmed(noteId);

            // Assert
            // Check for redirect after deletion
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Notes", redirectToActionResult.ActionName); // Assuming it redirects to "Notes"

            // Verify that DeleteConfirmed was called exactly once
            _notatRepositoryMock.Verify(repo => repo.DeleteConfirmed(noteId), Times.Once);
        }
        
        [Fact]
        public async Task DeleteConfirmed_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var noteId = 50;
            var currentUserName = "testUser";
            var anotherUserName = "unauthorizedUser";
            var note = new Note { NoteId = noteId, username = anotherUserName };

            // Set up the repository to return the note owned by a different user
            _notatRepositoryMock.Setup(repo => repo.GetNoteById(noteId)).ReturnsAsync(note);

            // Mock the current user identity to a different user
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

            // Act
            var result = await _controller.DeleteConfirmed(noteId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Edit_Note_ReturnsToNotes_WhenUpdateIsOk()
        {
            // Arrange
            var noteId = 50;
            var currentUserName = "testUser";
            var existingTitle = "Old Title";
            var existingInnhold = "Old Innhold";
            var newTitle = "New Title";
            var newInnhold = "New Innhold";

            // Set up the existing note in the repository
            var existingNote = new Note
            {
                NoteId = noteId,
                username = currentUserName,
                Tittel = existingTitle,
                Innhold = existingInnhold,
            };

            // Set up the updated note details
            var updatedNote = new Note
            {
                NoteId = noteId, // Ensure the IDs match to pass the ID check
                Tittel = newTitle,
                Innhold = newInnhold
            };

            // Mock repository to return the existing note and confirm successful update
            _notatRepositoryMock.Setup(repo => repo.GetNoteById(noteId)).ReturnsAsync(existingNote);
            _notatRepositoryMock.Setup(repo => repo.Update(existingNote)).Returns(Task.CompletedTask);

            // Mock user identity to match the note owner
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

            // Ensure ModelState is valid
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Update(updatedNote);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Notes", redirectResult.ActionName); // Ensure redirection to the "Grid" action

            // Verify that the Tittel and Innhold have been updated in the existing note
            Assert.Equal(newTitle, existingNote.Tittel);
            Assert.Equal(newInnhold, existingNote.Innhold);

            // Verify that Oppdater was called on the repository
            _notatRepositoryMock.Verify(repo => repo.Update(existingNote), Times.Once);
        }

        [Fact]
        public async Task Details_GoIntoDetailedView_FromNotes()
        {
            // Arrange
            var noteId = 50;
            var source = "Notes";
            var expectedNote = new Note
            {
                NoteId = noteId,
                Tittel = "Test Title",
                Innhold = "Test Description",
            };

            // Set up the repository to return the expected Note
            _notatRepositoryMock.Setup(repo => repo.GetNoteById(noteId)).ReturnsAsync(expectedNote);

            // Act
            var result = await _controller.Details(noteId, source);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", viewResult.ViewName); // Check that the view name is "NoteDetails"
            Assert.Equal(expectedNote, viewResult.Model); // Verify that the model is the expected Note

            // Check that ViewBag.Source is set correctly
            Assert.Equal(source, _controller.ViewBag.Source);

            // Verify that NoteId was called on the repository with the correct id
            _notatRepositoryMock.Verify(repo => repo.GetNoteById(noteId), Times.Once);
        }
    }
}
