using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Repositories.Repository;
using Shared.Dtos;
using Shared.Services;

namespace ApiTemplate.Controllers
{
    [ApiVersion(ApiVersioningConstants.CurrentVersion)]
    [Route(ApiVersioningConstants.versionRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AuthController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        /// <summary>
        /// Authenticates a user and returns a token.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var report = _unitOfWork.GetRepository<IAuthRepository>();
                var result = await report.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized("Invalid credentials or inactive account");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Login failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _unitOfWork.IAuthRepository.RegisterAsync(userDto);
                _unitOfWork.Commit();
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Changes the password for the current authenticated user.
        /// </summary>
        [Authorize]
        [EnableRateLimiting("FixedPolicy")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized("Invalid user session.");

                var result = await _unitOfWork.IAuthRepository.ChangePasswordAsync(userId, dto);
                if (!result)
                    return NotFound("User not found.");

                _unitOfWork.Commit();
                return Ok("Password changed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Error changing password: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Logged out successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Logout failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initiates password reset by generating a reset token.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resetToken = await _unitOfWork.IAuthRepository.GeneratePasswordResetTokenAsync(forgotPasswordDto);
                _unitOfWork.Commit();

                // Always return the same message for security (don't reveal if email exists)
                return Ok(new {
                    Message = "If your email exists in our system, you will receive a password reset link.",
                    EmailSent = !string.IsNullOrEmpty(resetToken)
                });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Failed to generate reset token: {ex.Message}");
            }
        }

        /// <summary>
        /// DEBUG ONLY: Gets password reset token for testing purposes.
        /// Remove this endpoint in production!
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("forgot-password-debug")]
        public async Task<IActionResult> ForgotPasswordDebug([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resetToken = await _unitOfWork.IAuthRepository.GeneratePasswordResetTokenAsync(forgotPasswordDto);
                _unitOfWork.Commit();

                if (string.IsNullOrEmpty(resetToken))
                {
                    return BadRequest("User not found or inactive.");
                }

                return Ok(new
                {
                    Message = "Reset token generated successfully (DEBUG MODE).",
                    ResetToken = resetToken,
                    Email = forgotPasswordDto.Email,
                    ExpiresInHours = 1,
                    Warning = "This endpoint is for testing only. Remove in production!"
                });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Failed to generate reset token: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets a user's password using a reset token.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _unitOfWork.IAuthRepository.ResetPasswordAsync(resetPasswordDto);
                if (!result)
                {
                    return BadRequest("Invalid reset token, token expired, or user not found.");
                }

                _unitOfWork.Commit();
                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Password reset failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a JWT reset token for the current authenticated user.
        /// </summary>
        [Authorize]
        [EnableRateLimiting("FixedPolicy")]
        [HttpPost("generate-jwt-reset-token")]
        public async Task<IActionResult> GenerateJwtResetToken()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized("Invalid user session.");

                var resetToken = await _unitOfWork.IAuthRepository.GenerateJwtResetTokenAsync(userId);
                if (string.IsNullOrEmpty(resetToken))
                {
                    return BadRequest("Unable to generate reset token for inactive user.");
                }

                _unitOfWork.Commit();
                return Ok(new
                {
                    Message = "JWT reset token has been sent to your registered email address.",
                    ExpiresInMinutes = 15,
                    EmailSent = true
                });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Failed to generate JWT reset token: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes JWT token using a reset token.
        /// </summary>
        [SkipJwtValidation]
        [EnableRateLimiting("PerIPPolicy")]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tokenResponse = await _unitOfWork.IAuthRepository.RefreshTokenWithResetTokenAsync(refreshTokenDto.ResetToken);
                if (tokenResponse == null)
                {
                    return BadRequest("Invalid or expired reset token.");
                }

                _unitOfWork.Commit();
                return Ok(new
                {
                    Message = "Token refreshed successfully.",
                    Token = tokenResponse.JWTToken
                });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return StatusCode(500, $"Token refresh failed: {ex.Message}");
            }
        }

        /// <summary>
        /// DEBUG ONLY: Test email sending functionality.
        /// </summary>
        [SkipJwtValidation]
        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailDto testEmailDto)
        {
            try
            {
                var emailRequest = new EmailRequest
                {
                    ToEmails = { testEmailDto.ToEmail },
                    Subject = "Test Email from TRACKIT API",
                    Body = $@"
                        <h2>Test Email</h2>
                        <p>This is a test email sent from TRACKIT API at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.</p>
                        <p>If you receive this email, your email configuration is working correctly!</p>
                        <hr>
                        <small>Test message sent to: {testEmailDto.ToEmail}</small>
                    ",
                    IsBodyHtml = true
                };

                var result = await _emailService.SendEmailAsync(emailRequest);

                return Ok(new
                {
                    Message = result ? "Test email sent successfully!" : "Failed to send test email",
                    EmailSent = result,
                    ToEmail = testEmailDto.ToEmail,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint for authentication.
        /// </summary>
        [Authorize]
        [HttpGet("test")]
        public IActionResult TestAuth()
        {
            return Ok(new
            {
                Message = "Authentication successful",
                User = User.Identity?.Name,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
