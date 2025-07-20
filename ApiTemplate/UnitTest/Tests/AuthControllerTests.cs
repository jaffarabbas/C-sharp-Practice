using ApiTemplate.Controllers;
using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repositories.Repository;
using Shared.Dtos;

namespace ApiTemplate.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAuthRepository> _authRepoMock;
        private Mock<IGenericRepository<TblUser>> _userRepoMock;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authRepoMock = new Mock<IAuthRepository>();
            _userRepoMock = new Mock<IGenericRepository<TblUser>>();

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<TblUser>()).Returns(_userRepoMock.Object);

            _controller = new AuthController(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task Login_ReturnsOk_WhenUserIsValid()
        {
            var loginDto = new LoginDto { Username = "test", Password = "pass" };
            var loginResponse = new LoginResponse { Token = "token" };
            _authRepoMock.Setup(r => r.Login(loginDto)).ReturnsAsync(loginResponse);

            var result = await _controller.Login(loginDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(loginResponse));
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenUserIsInvalid()
        {
            var loginDto = new LoginDto { Username = "test", Password = "wrong" };
            _authRepoMock.Setup(r => r.Login(loginDto))!.ReturnsAsync((LoginResponse?)null); // Fix for CS8620 and CS8600

            var result = await _controller.Login(loginDto);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Register_ReturnsOk_WhenUserIsValid()
        {
            var userDto = new TblUsersDto
            {
                Username = "user",
                Password = "pass",
                Firstname = "First",
                Lastname = "Last",
                Email = "email@test.com",
                AccountType = 1
            };
            _userRepoMock.Setup(r => r.GetMaxID("tblUsers", "Userid")).ReturnsAsync(1L);
            _userRepoMock.Setup(r => r.AddAsync("tblUsers", It.IsAny<TblUser>())).ReturnsAsync(1);

            var result = await _controller.Register(userDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult?.Value!.ToString(), Does.Contain("User Created Successfully"));
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenUserIsNull()
        {
            var result = await _controller.Register(null!);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("User cannot be null"));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenUserExists()
        {
            var user = new TblUser { Userid = 1, Username = "user" };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 1L)).ReturnsAsync(user);

            var result = await _controller.GetById(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(user));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 2L)).ReturnsAsync((TblUser)null);

            var result = await _controller.GetById(2);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithUsers()
        {
            var users = new List<TblUser> { new TblUser { Userid = 1, Username = "user" } };
            _userRepoMock.Setup(r => r.GetAllAsync("tblUser")).ReturnsAsync(users);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task UpdateProfile_ReturnsOk_WhenUserIsUpdated()
        {
            var userDto = new TblUsersDto
            {
                Username = "updated",
                Firstname = "First",
                Lastname = "Last",
                Email = "email@test.com",
                AccountType = 1
            };
            var user = new TblUser { Userid = 1, Username = "user" };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 1L)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync("tblUser", user, "UserID")).ReturnsAsync(1);

            var result = await _controller.UpdateProfile(1, userDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("User profile updated successfully."));
        }

        [Test]
        public async Task UpdateProfile_ReturnsBadRequest_WhenUserIsNull()
        {
            var result = await _controller.UpdateProfile(1, null);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Invalid user data."));
        }

        [Test]
        public async Task UpdateProfile_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userDto = new TblUsersDto();
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 2L)).ReturnsAsync((TblUser)null);

            var result = await _controller.UpdateProfile(2, userDto);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ChangePassword_ReturnsOk_WhenPasswordChanged()
        {
            var dto = new ChangePasswordDto { NewPassword = "newpass" };
            var user = new TblUser { Userid = 1, Username = "user" };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 1L)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync("tblUser", user, "UserID")).ReturnsAsync(1);

            var result = await _controller.ChangePassword(1, dto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Password changed successfully."));
        }

        [Test]
        public async Task ChangePassword_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.ChangePassword(1, null);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Invalid password data."));
        }

        [Test]
        public async Task ChangePassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var dto = new ChangePasswordDto { NewPassword = "newpass" };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 2L)).ReturnsAsync((TblUser)null);

            var result = await _controller.ChangePassword(2, dto);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeactivateUser_ReturnsOk_WhenUserDeactivated()
        {
            var user = new TblUser { Userid = 1, Username = "user", Status = true };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 1L)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync("tblUser", user, "UserID")).ReturnsAsync(1);

            var result = await _controller.DeactivateUser(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("User deactivated successfully."));
        }

        [Test]
        public async Task DeactivateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 2L)).ReturnsAsync((TblUser)null);

            var result = await _controller.DeactivateUser(2);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ReactivateUser_ReturnsOk_WhenUserReactivated()
        {
            var user = new TblUser { Userid = 1, Username = "user", Status = false };
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 1L)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync("tblUser", user, "UserID")).ReturnsAsync(1);

            var result = await _controller.ReactivateUser(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("User reactivated successfully."));
        }

        [Test]
        public async Task ReactivateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync("tblUser", "UserID", 2L)).ReturnsAsync((TblUser)null);

            var result = await _controller.ReactivateUser(2);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteUser_ReturnsOk_WhenUserDeleted()
        {
            _userRepoMock.Setup(r => r.DeleteAsync("tblUser", "UserID", 1L)).ReturnsAsync(1);

            var result = await _controller.DeleteUser(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("User deleted successfully."));
        }
    }
}