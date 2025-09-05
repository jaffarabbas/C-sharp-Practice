using ApiTemplate.Dtos;
using ApiTemplate.Helper;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Dtos;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Repositories.Repository
{
    public class AuthRepository : GenericRepository<LoginDto>, IAuthRepository
    {
        private readonly TestContext _context;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;
        private readonly IMemoryCache _cache;
        private readonly JWTSetting _setting;

        public AuthRepository(
            TestContext context,
            IDbConnection connection,
            IMemoryCache cache,
            IDbTransaction? transaction,
            IOptions<JWTSetting> settings) : base(context, connection, cache, transaction)
        {
            _context = context;
            _connection = connection;
            _transaction = transaction;
            _cache = cache;
            _setting = settings.Value;
        }

        public async Task<LoginResponse?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.TblUsers
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
                return null;

            var saltBytes = Convert.FromBase64String(user.Salt);
            var inputHash = HashPassword.Hash(loginDto.Password!, saltBytes);
            if (Convert.ToBase64String(inputHash) != user.Password)
                return null;

            var tokenResponse = GenerateToken(user);

            var roles = (from ur in _context.TblUserRoles
                         join r in _context.TblRoles on ur.RoleId equals r.RoleId
                         where ur.UserId == user.Userid
                         select r.RoleTitle).ToList();

            return new LoginResponse
            {
                Token = tokenResponse.JWTToken,
                LoginDate = DateTime.Now,
                Roles = roles
            };
        }

        public async Task<object> RegisterAsync(RegisterUserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentException("User cannot be null");

            // Manual repository instances (avoiding UnitOfWork dependency)
            var userRepo = new GenericRepository<TblUser>(_context, _connection, _cache, _transaction);
            var roleRepo = new GenericRepository<TblUserRole>(_context, _connection, _cache, _transaction);

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

            if (userDto.Roles != null && userDto.Roles.Any())
            {
                foreach (var roleDto in userDto.Roles)
                {
                    var userRole = new TblUserRole
                    {
                        UserRoleId = (int)await roleRepo.GetMaxID("tblUserRole", "UserRoleId"),
                        UserId = newUser.Userid,
                        RoleId = roleDto.RoleId,
                        UserRoleIsActive = true,
                        UserRoleCreatedAt = DateTime.UtcNow,
                    };
                    await roleRepo.AddAsync("tblUserRole", userRole);
                }
            }
            else
            {
                var defaultRole = new TblUserRole
                {
                    UserRoleId = (int)await roleRepo.GetMaxID("tblUserRole", "UserRoleId"),
                    UserId = newUser.Userid,
                    RoleId = 1,
                    UserRoleIsActive = true,
                    UserRoleCreatedAt = DateTime.UtcNow
                };
                await roleRepo.AddAsync("tblUserRole", defaultRole);
            }

            // No commit here; caller (controller/service) will call UnitOfWork.Commit()
            return new
            {
                Userid = newUser.Userid,
                Username = newUser.Username,
                Email = newUser.Email,
                Roles = userDto.Roles?.Select(r => r.RoleId).ToList() ?? new List<int> { 1 },
                UserCreatedDate = DateTime.UtcNow,
                Message = "User created successfully"
            };
        }

        private TokenResponse GenerateToken(TblUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_setting.securitykey);

            var roles = (from ur in _context.TblUserRoles
                         join r in _context.TblRoles on ur.RoleId equals r.RoleId
                         where ur.UserId == user.Userid
                         select r.RoleTitle).ToList();

            var claims = new List<Claim> { new(ClaimTypes.Name, user.Username!) };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            claims.Add(new Claim("userid",user.Userid.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {  
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResponse { JWTToken = tokenHandler.WriteToken(token) };
        }
    }
}
    