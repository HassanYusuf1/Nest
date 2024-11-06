using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using InstagramMVC.Controller;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using InstagramMVC.Models;

namespace InstagramMVC.Tests.Controller
{
public class BildeControllerTests : 
{
    private readonly Mock<IBildeRepository> _bildeRepositoryMock;
    private readonly Mock<ILogger<BildeController>> _loggerMock;
    private readonly BildeController _controller;

    public BildeControllerTests()
    {
        _bildeRepositoryMock = new Mock<IBildeRepository>();
        _loggerMock = new Mock<ILogger<BildeController>>();
        _controller = new BildeController(_bildeRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Grid_ReturnsNotFound_WhenBilderIsNull()
    {
        // Arrange
        _bildeRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync((List<Bilde>)null);

        // Act
        var result = await _controller.Grid();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Bildene ble ikke funnet", notFoundResult.Value);

        _loggerMock.Verify(
            logger => logger.LogError(It.Is<string>(s => s.Contains("Bilde liste, ble ikke funnet."))),
            Times.Once);
    }

    [Fact]
    public async Task Grid_ReturnsViewResult_WithBilderViewModel()
    {
        // Arrange
        var bilderList = new List<Bilde> { new Bilde() }; // Mock a list of "Bilde" objects
        _bildeRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(bilderList);

        // Act
        var result = await _controller.Grid();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BilderViewModel>(viewResult.Model);
        Assert.Equal(bilderList, model.Bilder);
        Assert.Equal("Bilde", model.Title);
    }
}
}
