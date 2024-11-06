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

public class BildeControllerTests
{
    private readonly Mock<IBildeRepository> _bildeRepositoryMock;
    private readonly Mock<IKommentarRepository> _kommentarRepositoryMock;
    private readonly Mock<ILogger<BildeController>> _loggerMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IUrlHelper> _urlHelperMock;
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

        _urlHelperMock = new Mock<IUrlHelper>();


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

    [Fact]
    public async Task DeleteConfirmed_DeletesFile_WhenFileExists()
    {
        // Arrange
        var imageId = 50;
        var bildeUrl = "/images/test.jpg";
        var currentUserName = "testUser";
        var returnUrl = "https://localhost/Grid";
        var bilde = new Bilde { BildeId = imageId, BildeUrl = bildeUrl, UserName = currentUserName };
        
        // Set up the repository to return the image and confirm deletion
        _bildeRepositoryMock.Setup(repo => repo.BildeId(imageId)).ReturnsAsync(bilde);
        _bildeRepositoryMock.Setup(repo => repo.Delete(imageId)).ReturnsAsync(true);
        
        // Mock user identity
        _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

        _urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns(returnUrl);
        _controller.Url = _urlHelperMock.Object;

        
        // Set up the delegates to simulate file existence and track deletion
        bool fileDeleted = false;
        FileUtil.FileExists = path => true; // Simulate that the file exists
        FileUtil.FileDelete = path => fileDeleted = true; // Track if deletion occurs

        // Act
        var result = await _controller.DeleteConfirmed(imageId);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result); // Expect RedirectResult
        Assert.Equal(returnUrl, redirectResult.Url); // Check that the URL matches the expected returnUrl
        Assert.True(fileDeleted); // Verify that the file was deleted
        _bildeRepositoryMock.Verify(repo => repo.Delete(imageId), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ReturnsNotFound_WhenBildeDoesNotExist()
    {
        // Arrange
        var imageId = 100;

        // Set up the repository to return null for a non-existing image
        _bildeRepositoryMock.Setup(repo => repo.BildeId(imageId)).ReturnsAsync((Bilde)null);

        // Act
        var result = await _controller.DeleteConfirmed(imageId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ReturnsForbid_WhenUserIsNotAuthorized()
    {
        // Arrange
        var imageId = 50;
        var bildeUrl = "/images/test.jpg";
        var currentUserName = "testUser";
        var anotherUserName = "unauthorizedUser";
        var bilde = new Bilde { BildeId = imageId, BildeUrl = bildeUrl, UserName = anotherUserName };

        // Set up the repository to return the image owned by a different user
        _bildeRepositoryMock.Setup(repo => repo.BildeId(imageId)).ReturnsAsync(bilde);

        // Mock the current user identity to a different user
        _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

        // Act
        var result = await _controller.DeleteConfirmed(imageId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    // Reset FileUtil delegates after each test to avoid side effects in other tests
    public void Dispose()
    {
        FileUtil.FileExists = System.IO.File.Exists;
        FileUtil.FileDelete = System.IO.File.Delete;
    }

    [Fact]
    public async Task Edit_image_ReturnsToGrid_WhenUpdateIsOk()
    {
        // Arrange
        var imageId = 50;
        var currentUserName = "testUser";
        var existingTitle = "Old Title";
        var existingDescription = "Old Description";
        var newTitle = "New Title";
        var newDescription = "New Description";

        // Set up the existing image in the repository
        var existingBilde = new Bilde
        {
            BildeId = imageId,
            UserName = currentUserName,
            Tittel = existingTitle,
            Beskrivelse = existingDescription,
            BildeUrl = "/images/oldImage.jpg"
        };

        // Set up the updated image details
        var updatedBilde = new Bilde
        {
            BildeId = imageId, // Ensure the IDs match to pass the ID check
            Tittel = newTitle,
            Beskrivelse = newDescription
        };

        // Mock repository to return the existing image and confirm successful update
        _bildeRepositoryMock.Setup(repo => repo.BildeId(imageId)).ReturnsAsync(existingBilde);
        _bildeRepositoryMock.Setup(repo => repo.Oppdater(existingBilde)).ReturnsAsync(true);

        // Mock user identity to match the image owner
        _userManagerMock.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns(currentUserName);

        // Ensure ModelState is valid
        _controller.ModelState.Clear();

        // Act
        var result = await _controller.Edit(imageId, updatedBilde, null);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Grid", redirectResult.ActionName); // Ensure redirection to the "Grid" action

        // Verify that the Tittel and Beskrivelse have been updated in the existing image
        Assert.Equal(newTitle, existingBilde.Tittel);
        Assert.Equal(newDescription, existingBilde.Beskrivelse);

        // Verify that Oppdater was called on the repository
        _bildeRepositoryMock.Verify(repo => repo.Oppdater(existingBilde), Times.Once);
    }

}

