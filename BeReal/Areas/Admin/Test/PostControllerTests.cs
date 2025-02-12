using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Areas.Admin.Controllers;
using BeReal.ViewModels;
using BeReal.Data.Repository.Files;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;
using BeReal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.ContentModel;

public class PostControllerTests
{
    private readonly Mock<IPostsOperations> _mockPostsOperations;
    private readonly Mock<IUsersOperations> _mockUsersOperations;
    private readonly Mock<IFileManager> _mockFileManager;
    private readonly Mock<INotyfService> _mockNotification;
    private readonly PostController _controller;

    public PostControllerTests()
    {
        _mockPostsOperations = new Mock<IPostsOperations>();
        _mockUsersOperations = new Mock<IUsersOperations>();
        _mockFileManager = new Mock<IFileManager>();
        _mockNotification = new Mock<INotyfService>();

        _controller = new PostController(_mockNotification.Object, _mockPostsOperations.Object, _mockUsersOperations.Object, _mockFileManager.Object);
    }

    [Fact]
    public async Task Create_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var model = new CreatePostViewModel
        {
            Title = "Test Title",
            ShortDescription = "Test Description",
            Category = "Test Category",
            Description = "Test Content"
        };

        var user = new BR_ApplicationUser { Id = "1", FirstName = "John", LastName = "Doe" };
        _mockUsersOperations.Setup(u => u.GetLoggedUser(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

        var post = new BR_Post { IDBR_Post = 1, Title = model.Title, Author = "John Doe" };
        _mockPostsOperations.Setup(p => p.GetPostValues(It.IsAny<BR_Post>(), It.IsAny<CreatePostViewModel>(), It.IsAny<BR_ApplicationUser>(), It.IsAny<IUsersOperations>())).ReturnsAsync(post);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName);
    }
}
