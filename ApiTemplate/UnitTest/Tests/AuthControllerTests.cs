using ApiTemplate.Controllers;
using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories.Repository;
using Shared.Dtos;
using Shared.Services;
using System.Security.Claims;

namespace ApiTemplate.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAuthRepository> _authRepoMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IAuditLoggingService> _auditLoggerMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authRepoMock = new Mock<IAuthRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _auditLoggerMock = new Mock<IAuditLoggingService>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            _unitOfWorkMock.Setup(u => u.GetRepository<IAuthRepository>()).Returns(_authRepoMock.Object);

            _controller = new AuthController(
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _auditLoggerMock.Object,
                _loggerMock.Object
            );

            // Setup HttpContext with a valid IP address
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Test]
        public async Task Login_ReturnsOk_WhenUserIsValid()
        {
            var loginDto = new LoginDto { Username = "test", Password = "pass" };
            var loginResponse = new LoginResponse { Token = "token" };
            _authRepoMock.Setup(r => r.LoginAsync(loginDto)).ReturnsAsync(loginResponse);

            var result = await _controller.Login(loginDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(loginResponse));
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenUserIsInvalid()
        {
            var loginDto = new LoginDto { Username = "test", Password = "wrong" };
            _authRepoMock.Setup(r => r.LoginAsync(loginDto))!.ReturnsAsync((LoginResponse?)null); // Fix for CS8620 and CS8600

            var result = await _controller.Login(loginDto);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Register_ReturnsOk_WhenUserIsValid()
        {
            var registerUserDto = new RegisterUserDto
            {
                Username = "user",
                Password = "ValidP@ss123",
                Firstname = "First",
                Lastname = "Last",
                Email = "email@test.com"
            };
            var expectedResponse = new { Message = "User created successfully" };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.RegisterAsync(registerUserDto)).ReturnsAsync(expectedResponse);

            var result = await _controller.Register(registerUserDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenPasswordPolicyViolated()
        {
            var registerUserDto = new RegisterUserDto
            {
                Username = "test",
                Password = "weak",
                Email = "test@test.com",
                Firstname = "Test",
                Lastname = "User"
            };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.RegisterAsync(registerUserDto))
                .ThrowsAsync(new ArgumentException("Password does not meet policy requirements: Password too short"));

            var result = await _controller.Register(registerUserDto);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value?.ToString(), Does.Contain("Password does not meet policy requirements"));
            _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        }


        [Test]
        public async Task ChangePassword_ReturnsOk_WhenPasswordChanged()
        {
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "OldP@ss123",
                NewPassword = "NewP@ss123"
            };

            // Setup authenticated user context
            var claims = new[] { new Claim("userId", "1") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ChangePasswordAsync(1L, dto)).ReturnsAsync(true);

            var result = await _controller.ChangePassword(dto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Password changed successfully."));
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserIdClaimMissing()
        {
            var dto = new ChangePasswordDto { NewPassword = "NewP@ss123" };

            var result = await _controller.ChangePassword(dto);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.That(unauthorizedResult.Value, Is.EqualTo("Invalid user session."));
        }

        [Test]
        public async Task ChangePassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var dto = new ChangePasswordDto { NewPassword = "NewP@ss123" };

            // Setup authenticated user context
            var claims = new[] { new Claim("userId", "999") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ChangePasswordAsync(999L, dto)).ReturnsAsync(false);

            var result = await _controller.ChangePassword(dto);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.That(notFoundResult.Value, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task ChangePassword_ReturnsBadRequest_WhenPasswordPolicyViolated()
        {
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "OldP@ss123",
                NewPassword = "weak"
            };

            // Setup authenticated user context
            var claims = new[] { new Claim("userId", "1") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ChangePasswordAsync(1L, dto))
                .ThrowsAsync(new ArgumentException("New password does not meet policy requirements: Password too short"));

            var result = await _controller.ChangePassword(dto);

            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value?.ToString(), Does.Contain("New password does not meet policy requirements"));
            _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        }


        [Test]
        public async Task Register_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var registerUserDto = new RegisterUserDto
            {
                Username = "test",
                Email = "test@test.com",
                Password = "ValidP@ss123",
                Firstname = "Test",
                Lastname = "User"
            };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.RegisterAsync(registerUserDto))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _controller.Register(registerUserDto);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
            Assert.That(statusCodeResult.Value?.ToString(), Does.Contain("Database error"));
            _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        }

        [Test]
        public async Task Logout_ReturnsSuccess()
        {
            var result = await _controller.Logout() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Logged out successfully.", result.Value);
        }

        [Test]
        public async Task ForgotPassword_ReturnsOk()
        {
            var forgotPasswordDto = new ForgotPasswordDto { Email = "test@test.com" };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.GeneratePasswordResetTokenAsync(forgotPasswordDto))
                .ReturnsAsync("reset-token-123");

            var result = await _controller.ForgotPassword(forgotPasswordDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public async Task ResetPassword_ReturnsOk_WhenTokenIsValid()
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@test.com",
                ResetToken = "valid-token",
                NewPassword = "NewP@ss123"
            };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ResetPasswordAsync(resetPasswordDto)).ReturnsAsync(true);

            var result = await _controller.ResetPassword(resetPasswordDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Password reset successfully."));
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenIsInvalid()
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@test.com",
                ResetToken = "invalid-token",
                NewPassword = "NewP@ss123"
            };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ResetPasswordAsync(resetPasswordDto)).ReturnsAsync(false);

            var result = await _controller.ResetPassword(resetPasswordDto);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Invalid reset token, token expired, or user not found."));
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordPolicyViolated()
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@test.com",
                ResetToken = "valid-token",
                NewPassword = "weak"
            };

            _unitOfWorkMock.Setup(u => u.IAuthRepository).Returns(_authRepoMock.Object);
            _authRepoMock.Setup(r => r.ResetPasswordAsync(resetPasswordDto))
                .ThrowsAsync(new ArgumentException("New password does not meet policy requirements: Password too short"));

            var result = await _controller.ResetPassword(resetPasswordDto);

            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value?.ToString(), Does.Contain("New password does not meet policy requirements"));
            _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        }
    }
}