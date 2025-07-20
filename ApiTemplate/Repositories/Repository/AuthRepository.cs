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

        public AuthRepository(TestContext context, IDbConnection connection, IMemoryCache cache, IDbTransaction? transaction, IOptions<JWTSetting> settings) : base(context, connection, cache, transaction)
        {
            _context = context;
            _connection = connection;
            _transaction = transaction;
            _cache = cache;
            _setting = settings.Value;
        }

        public async Task<LoginResponse?> Login(LoginDto loginDto)
        {
            try
            {
                var _user = await _context.TblUsers.FirstOrDefaultAsync(data => data.Username == loginDto.Username);
                if (_user == null)
                {
                    return null;
                }
                else
                {
                    var saltBytes = Convert.FromBase64String(_user.Salt);
                    var inputHash = HashPassword.Hash(loginDto.Password!, saltBytes);
                    bool isValid = Convert.ToBase64String(inputHash) == _user.Password;
                    if (isValid) { 
                        var responce = GenerateToken(_user!);
                        var loginResponse = new LoginResponse()
                        {
                            Token = responce.JWTToken,
                            LoginDate = DateTime.Now,
                            UserType = _user.AccountType.ToString(),
                        };
                        return loginResponse;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception error)
            {
                throw;
            }
        }

        private TokenResponse GenerateToken(TblUser user)
        {
            var tokenResponce = new TokenResponse();
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(this._setting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name,user.Username!),
                    }
                ),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenHandler.WriteToken(token);
            tokenResponce.JWTToken = finaltoken;
            return tokenResponce;
        }
    }
}
