using ApiTemplate.Dtos;
using ApiTemplate.Helper;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.Attributes;
using Repositories.Services;
using Shared.Dtos;
using Shared.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Repositories.Repository
{
    [AutoRegisterRepository(typeof(IAuthRepository))]
    public class AuthRepository : BaseRepository<LoginDto>, IAuthRepository
    {
        private readonly TestContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JWTSetting _setting;

        // Lazy-loaded services - only resolved when needed
        private IEmailService? _emailService;
        private IEmailService EmailService => _emailService ??= GetService<IEmailService>();

        private IPasswordPolicyService? _passwordPolicyService;
        private IPasswordPolicyService PasswordPolicyService => _passwordPolicyService ??= GetService<IPasswordPolicyService>();

        public AuthRepository(
            TestContext context,
            IDbConnection connection,
            IMemoryCache cache,
            IDbTransaction? transaction,
            IOptions<JWTSetting> settings,
            IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider) : base(context, connection, cache, transaction, serviceProvider)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _setting = settings.Value;
        }

        public async Task<LoginResponse?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<TblUser>();
                var user = await _context.TblUsers
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null || !user.Status)
                    return null;

                var saltBytes = Convert.FromBase64String(user.Salt);
                var inputHash = HashPassword.Hash(loginDto.Password!, saltBytes);
                if (Convert.ToBase64String(inputHash) != user.Password)
                    return null;

                var tokenResponse = GenerateToken(user);
                var roles = await GetUserRolesAsync(user.Userid);

                return new LoginResponse
                {
                    Token = tokenResponse.JWTToken,
                    LoginDate = DateTime.UtcNow,
                    Roles = roles.ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> RegisterAsync(RegisterUserDto userDto)
        {
            try
            {
                if (userDto == null)
                    throw new ArgumentException("User cannot be null");

                // Validate password against policy (using companyId = 1 as default, adjust as needed)
                const long defaultCompanyId = 1;
                var passwordValidation = await PasswordPolicyService.ValidatePasswordAsync(userDto.Password, defaultCompanyId);

                if (!passwordValidation.IsValid)
                {
                    throw new ArgumentException($"Password does not meet policy requirements: {string.Join(", ", passwordValidation.Errors)}");
                }

                // Check if username or email already exists
                var existingUser = await _context.TblUsers
                    .FirstOrDefaultAsync(u => u.Username == userDto.Username || u.Email == userDto.Email);

                if (existingUser != null)
                    throw new ArgumentException("Username or email already exists");

                var userRepo = _unitOfWork.Repository<TblUser>();
                var roleRepo = _unitOfWork.Repository<TblUserRole>();

                // Get next IDs
                long newUserId = await userRepo.GetMaxID("tblUsers", "Userid");

                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(userDto.Password, salt, "sha256");

                var newUser = new TblUser
                {
                    Userid = newUserId,
                    Username = userDto.Username,
                    Firstname = userDto.Firstname,
                    Lastname = userDto.Lastname,
                    Email = userDto.Email,
                    Password = Convert.ToBase64String(encryptedPassword),
                    Salt = Convert.ToBase64String(salt),
                    CreatedAt = DateTime.UtcNow,
                    Status = true
                };

                await userRepo.AddAsync("tblUsers", newUser);

                // Handle roles
                var roleIds = new List<int>();
                if (userDto.Roles != null && userDto.Roles.Any())
                {
                    // Get the starting ID ONCE before the loop
                    long userRoleId = await roleRepo.GetMaxID("tblUserRole", "UserRoleId");

                    foreach (var roleDto in userDto.Roles)
                    {
                        var userRole = new TblUserRole
                        {
                            UserRoleId = (int)userRoleId,
                            UserId = newUser.Userid,
                            RoleId = roleDto.RoleId,
                            UserRoleIsActive = true,
                            UserRoleCreatedAt = DateTime.UtcNow,
                        };
                        await roleRepo.AddAsync("tblUserRole", userRole);
                        roleIds.Add(roleDto.RoleId);

                        // Increment ID for next iteration
                        userRoleId++;
                    }
                }
                else
                {
                    // Assign default role (assuming role ID 1 is a default role like "User")
                    long userRoleId = await roleRepo.GetMaxID("tblUserRole", "UserRoleId");
                    var defaultRole = new TblUserRole
                    {
                        UserRoleId = (int)userRoleId,
                        UserId = newUser.Userid,
                        RoleId = 1, // Default role
                        UserRoleIsActive = true,
                        UserRoleCreatedAt = DateTime.UtcNow
                    };
                    await roleRepo.AddAsync("tblUserRole", defaultRole);
                    roleIds.Add(1);
                }

                return new
                {
                    UserId = newUser.Userid,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    Firstname = newUser.Firstname,
                    Lastname = newUser.Lastname,
                    Roles = roleIds,
                    CreatedDate = newUser.CreatedAt,
                    Message = "User created successfully"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto passwordDto)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<TblUser>();
                var user = await userRepo.GetByIdAsync("tblUsers", "Userid", userId);

                if (user == null)
                    return false;

                // Verify current password if provided
                if (!string.IsNullOrWhiteSpace(passwordDto.CurrentPassword))
                {
                    var currentSalt = Convert.FromBase64String(user.Salt);
                    var currentHashedPassword = HashPassword.Hash(passwordDto.CurrentPassword, currentSalt, "sha256");
                    if (user.Password != Convert.ToBase64String(currentHashedPassword))
                        throw new UnauthorizedAccessException("Current password is incorrect.");
                }

                // Validate new password against policy
                const long defaultCompanyId = 1;
                var passwordValidation = await PasswordPolicyService.ValidatePasswordAsync(passwordDto.NewPassword, defaultCompanyId);

                if (!passwordValidation.IsValid)
                {
                    throw new ArgumentException($"New password does not meet policy requirements: {string.Join(", ", passwordValidation.Errors)}");
                }

                // Update password
                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(passwordDto.NewPassword, salt, "sha256");
                user.Password = Convert.ToBase64String(encryptedPassword);
                user.Salt = Convert.ToBase64String(salt);

                await userRepo.UpdateAsync("tblUsers", user, "Userid");
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_setting.securitykey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = await tokenHandler.ValidateTokenAsync(token, validationParameters);
                return principal.IsValid;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(long userId)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<TblUser>();
                var user = await userRepo.GetByIdAsync("tblUsers", "Userid", userId);

                if (user == null || !user.Status)
                    throw new UnauthorizedAccessException("User not found or inactive");

                return GenerateToken(user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        private TokenResponse GenerateToken(TblUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_setting.securitykey);

            var roles = (from ur in _context.TblUserRoles
                         join r in _context.TblRoles on ur.RoleId equals r.RoleId
                         where ur.UserId == user.Userid && ur.UserRoleIsActive
                         select r.RoleTitle).ToList();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username!),
                new("userId", user.Userid.ToString()),
                new("email", user.Email ?? ""),
                new("fullName", $"{user.Firstname} {user.Lastname}".Trim())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(10),
                Issuer = _setting.ValidIssuer,
                Audience = _setting.ValidAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResponse
            {
                JWTToken = tokenHandler.WriteToken(token)
            };
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _context.TblUsers
                    .FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email && u.Status);

                if (user == null)
                    return string.Empty; // Don't reveal if email exists or not

                // Invalidate any existing password reset tokens for this user
                var existingTokens = await _context.TblResetTokens
                    .Where(t => t.UserId == user.Userid && t.TokenType == "PASSWORD_RESET" && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var token in existingTokens)
                {
                    token.IsUsed = true;
                }

                // Generate new reset token
                var resetToken = Guid.NewGuid().ToString("N");
                var tokenRepo = _unitOfWork.Repository<TblResetToken>();
                var resetTokenId = await tokenRepo.GetMaxID("tblResetToken", "ResetTokenId");

                var passwordResetToken = new TblResetToken
                {
                    ResetTokenId = (int)resetTokenId,
                    UserId = user.Userid,
                    TokenType = "PASSWORD_RESET",
                    Token = resetToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                await tokenRepo.AddAsync("tblResetToken", passwordResetToken);

                // Send password reset email
                var userName = $"{user.Firstname} {user.Lastname}".Trim();
                await EmailService.SendPasswordResetEmailAsync(user.Email, userName, resetToken);

                return resetToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // Find and validate the reset token
                var resetTokenRecord = await _context.TblResetTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Token == resetPasswordDto.ResetToken
                        && t.TokenType == "PASSWORD_RESET"
                        && !t.IsUsed
                        && t.ExpiresAt > DateTime.UtcNow
                        && t.User.Email == resetPasswordDto.Email
                        && t.User.Status);

                if (resetTokenRecord == null)
                    return false;

                // Validate new password against policy
                const long defaultCompanyId = 1;
                var passwordValidation = await PasswordPolicyService.ValidatePasswordAsync(resetPasswordDto.NewPassword, defaultCompanyId);

                if (!passwordValidation.IsValid)
                {
                    throw new ArgumentException($"New password does not meet policy requirements: {string.Join(", ", passwordValidation.Errors)}");
                }

                var userRepo = _unitOfWork.Repository<TblUser>();
                var user = resetTokenRecord.User;

                // Update password
                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(resetPasswordDto.NewPassword, salt, "sha256");
                user.Password = Convert.ToBase64String(encryptedPassword);
                user.Salt = Convert.ToBase64String(salt);

                await userRepo.UpdateAsync("tblUsers", user, "Userid");

                // Mark token as used
                var tokenRepo = _unitOfWork.Repository<TblResetToken>();
                resetTokenRecord.IsUsed = true;
                await tokenRepo.UpdateAsync("tblResetToken", resetTokenRecord, "ResetTokenId");

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GenerateJwtResetTokenAsync(long userId)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<TblUser>();
                var user = await userRepo.GetByIdAsync("tblUsers", "Userid", userId);

                if (user == null || !user.Status)
                    return string.Empty;

                // Invalidate any existing JWT reset tokens for this user
                var existingTokens = await _context.TblResetTokens
                    .Where(t => t.UserId == userId && t.TokenType == "JWT_RESET" && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var token in existingTokens)
                {
                    token.IsUsed = true;
                }

                // Generate new JWT reset token
                var jwtResetToken = Guid.NewGuid().ToString("N");
                var tokenRepo = _unitOfWork.Repository<TblResetToken>();
                var resetTokenId = await tokenRepo.GetMaxID("tblResetToken", "ResetTokenId");

                var resetTokenRecord = new TblResetToken
                {
                    ResetTokenId = (int)resetTokenId,
                    UserId = userId,
                    TokenType = "JWT_RESET",
                    Token = jwtResetToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15), // JWT reset token expires in 15 minutes
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                await tokenRepo.AddAsync("tblResetToken", resetTokenRecord);

                // Send JWT reset token email
                var userName = $"{user.Firstname} {user.Lastname}".Trim();
                await EmailService.SendJwtResetTokenEmailAsync(user.Email, userName, jwtResetToken);

                return jwtResetToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TokenResponse?> RefreshTokenWithResetTokenAsync(string resetToken)
        {
            try
            {
                // Find and validate the JWT reset token
                var resetTokenRecord = await _context.TblResetTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Token == resetToken
                        && t.TokenType == "JWT_RESET"
                        && !t.IsUsed
                        && t.ExpiresAt > DateTime.UtcNow
                        && t.User.Status);

                if (resetTokenRecord == null)
                    return null;

                // Generate new JWT token
                var newJwtToken = GenerateToken(resetTokenRecord.User);

                // Mark reset token as used
                var tokenRepo = _unitOfWork.Repository<TblResetToken>();
                resetTokenRecord.IsUsed = true;
                await tokenRepo.UpdateAsync("tblResetToken", resetTokenRecord, "ResetTokenId");

                return newJwtToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<IEnumerable<string>> GetUserRolesAsync(long userId)
        {
            return await (from ur in _context.TblUserRoles
                         join r in _context.TblRoles on ur.RoleId equals r.RoleId
                         where ur.UserId == userId && ur.UserRoleIsActive
                         select r.RoleTitle).ToListAsync();
        }
    }
}