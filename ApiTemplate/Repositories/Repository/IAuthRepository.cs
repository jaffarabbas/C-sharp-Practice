using ApiTemplate.Dtos;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public interface IAuthRepository
    {
        public Task<LoginResponse> Login(LoginDto loginDto);
    }
}
