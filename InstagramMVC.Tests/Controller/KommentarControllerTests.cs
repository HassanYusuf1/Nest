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

public class CommentControllerTests
{
    private readonly Mock<IPictureRepository> _pictureRepositoryMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<INoteRepository> _noteRepositoryMock;
    private readonly Mock<ILogger<CommentController>> _loggerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IUrlHelper> _urlHelperMock;
    private readonly CommentController _controller;

    public CommentControllerTests() //This part of testing code is to initialize so we don't have to write this multiple times
    {
        _commentRepositoryMock = new Mock<ICommentRepository>(); 
        _pictureRepositoryMock = new Mock<IPictureRepository>();
        _noteRepositoryMock = new Mock<INoteRepository>();
        _loggerMock = new Mock<ILogger<CommentController>>();

        //Set up UserManager<IdentityUser> mock with default constructor parameters
        var store = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);


        //Pass all five required parameters to the CommentController constructor
        _controller = new CommentController(
            _commentRepositoryMock.Object, 
            _loggerMock.Object,
            _userManagerMock.Object);
    }

    [Fact]
    public async Task CreateCommentForPicture_SaveCommentInDB_Verifies()
    {
        // Arrange
        var testComment = new Comment
        {
            CommentId = 1,
            PictureId = 10,
            CommentDescription = "This is a test comment",
            CommentTime = DateTime.UtcNow,
            UserName = "TestUser"
        };

        _commentRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask); // Simulate repository behavior

        // Act
        var result = await _controller.CreateComment(testComment);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Grid", redirectResult.ActionName); // Change "Grid" to the appropriate action name if needed

        _commentRepositoryMock.Verify(repo => repo.Create(It.Is<Comment>(n => 
                n.CommentId == testComment.CommentId && 
                n.PictureId == testComment.PictureId && n.NoteId == null)), Times.Once); //NoteId has to be null
    }

     [Fact]
    public async Task CreateCommentForNotes_SaveCommentInDB_Verifies()
    {
        // Arrange
        var testComment = new Comment
        {
            CommentId = 1,
            NoteId = 10,
            CommentDescription = "This is a test comment",
            CommentTime = DateTime.UtcNow,
            UserName = "TestUser"
        };

        _commentRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask); // Simulate repository behavior

        // Act
        var result = _controller.CreateCommentNote(testComment);

        // Assert
        var redirectResult = Assert.IsType<ViewResult>(result); // This retrieves the ViewResult instance
        Assert.Equal(testComment, redirectResult.Model); // Use the instance to access the Model property


        _commentRepositoryMock.Verify(repo => repo.Create(It.Is<Comment>(n => 
                n.CommentId == testComment.CommentId && 
                n.NoteId == testComment.NoteId && n.PictureId == null)), Times.Once); //PictureId is null
    }
}