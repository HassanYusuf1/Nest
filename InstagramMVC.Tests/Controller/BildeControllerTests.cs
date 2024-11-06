using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using Moq;
using Xunit;
using InstagramMVC.Controllers; // Adjust namespace as needed
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;

namespace InstagramMVC.Tests.Controllers;

public class BildeControllerTests
{
    private readonly Mock<IBildeRepository> _bildeRepositoryMock;
    private readonly Mock<IKommentarRepository> _kommentarRepositoryMock;
    private readonly Mock<ILogger<BildeController>> _loggerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly BildeController _controller;

    public BildeControllerTests()
    {
        _bildeRepositoryMock = new Mock<IBildeRepository>();
        _kommentarRepositoryMock = new Mock<IKommentarRepository>(); // New mock for IKommentarRepository
        _loggerMock = new Mock<ILogger<BildeController>>();

        // Set up UserManager<IdentityUser> mock with default constructor parameters
        var store = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // Pass all four required parameters to the BildeController constructor
        _controller = new BildeController(
            _bildeRepositoryMock.Object,
            _kommentarRepositoryMock.Object, // New parameter for IKommentarRepository
            _loggerMock.Object,
            _userManagerMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Tittel", "Required");
        var newImage = new Bilde();

        // Act
        var result = await _controller.Create(newImage, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(newImage, viewResult.Model);
    }

    [Fact]
    public async Task Create_SavesImageFileAndCreatesDatabaseRecord_WhenValid()
    {
        // Arrange
        var newImage = new Bilde();
        var mockFile = new Mock<IFormFile>();
        var fileName = "test.jpg";
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(1024); // Set a file size

        // Setting up a mock file stream
        using (var ms = new MemoryStream())
        {
            mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var userName = "testUser";
            _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(userName);

            _bildeRepositoryMock.Setup(repo => repo.Create(It.IsAny<Bilde>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(newImage, mockFile.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyPage", redirectResult.ActionName);
            Assert.StartsWith("/images/", newImage.BildeUrl);
            Assert.EndsWith(".jpg", newImage.BildeUrl);
            _bildeRepositoryMock.Verify(repo => repo.Create(It.Is<Bilde>(b => b == newImage)), Times.Once);
        }
    }

    [Fact]
    public async Task Create_ReturnsView_WhenDatabaseSaveFails()
    {
        // Arrange
        var newImage = new Bilde();
        var mockFile = new Mock<IFormFile>();
        _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("testUser");
        _bildeRepositoryMock.Setup(repo => repo.Create(It.IsAny<Bilde>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Create(newImage, mockFile.Object);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(newImage, viewResult.Model);
    }

}

