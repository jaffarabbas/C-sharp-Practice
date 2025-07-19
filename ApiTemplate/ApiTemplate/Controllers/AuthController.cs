using ApiTemplate.Dtos;
using ApiTemplate.Helper;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Collections.Generic;

namespace ApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SkipJwtValidation]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var data = await _unitOfWork.IAuthRepository.Login(loginDto);
            if (data == null) {
                return Unauthorized("Invalid User");
            }
            return Ok(data);
        }
        [HttpPost("")]
        public async Task<IActionResult> Register([FromBody] TblUsersDto user)
        {
            if (user == null)
                return BadRequest("User cannot be null");

            try
            {
                //hashing password
                var salt = HashPassword.GenerateSalt();
                var encryptedPassword = HashPassword.Hash(user.Password,salt, "sha256");
                var model = new TblUser
                {
                    Userid = user.Userid,
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Password = Convert.ToBase64String(encryptedPassword),
                    Salt = Convert.ToBase64String(salt),
                    AccountType = user.AccountType,
                    CreatedAt = DateTime.UtcNow,
                    Status = user.Status,
                };
                var repo = _unitOfWork.Repository<TblUser>();
                await repo.AddAsync("tblUsers", model);
                _unitOfWork.Commit();
                return Ok(new
                {
                    Userid = user.Userid,
                    UserCreatedDate = DateTime.UtcNow,
                    Message = "User Created Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TblUser>();
                var data = await repo.GetByIdAsync("tblUser", "UserID", id);
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}