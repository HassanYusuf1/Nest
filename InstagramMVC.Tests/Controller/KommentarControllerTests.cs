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
    private readonly Mock<ILogger<PictureController>> _loggerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IUrlHelper> _urlHelperMock;
    private readonly CommentController _controller;

    public CommentControllerTests() //This part of testing code is to initialize so we don't have to write this multiple times
    {
        _commentRepositoryMock = new Mock<ICommentRepository>(); 
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
    public async Task CreateComment_ForPictures_CreateDatabaseAndNoNoteID()
    {
        //Arrange
        int pictureId = 1;
        var title = "hello";
        var description = "world";
        var pictureUrl = "/images/testImage.jpg";

        var userName = "testUser";
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(userName);

        var picture = new Picture { PictureId = pictureId, Title = title, Description = description, PictureUrl = pictureUrl, Comments = new List<Comment>(), UserName = userName };
            _pictureRepositoryMock.Setup(repo => repo.Create(It.IsAny<Picture>())).ReturnsAsync(true);

        


        //Act
        var result = _controller.CreateComment(pictureId);

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Comment>(viewResult.Model);
        Assert.Equal(pictureId, model.PictureId);
        Assert.Null(model.NoteId);
        Assert.Contains(model, picture.Comments);

        //Check if it has entered the database correctly
        _commentRepositoryMock.Verify(r => r.Create(It.Is<Comment>(c => c.PictureId == pictureId && c.NoteId == null)), Times.Once);


    }
}