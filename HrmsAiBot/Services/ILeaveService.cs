using Dapper;
using Microsoft.Data.SqlClient;

namespace Service
{
    public interface ILeaveService
    {
        Task<int> GetBalanceAsync(int userId);
    }

    public class LeaveService : ILeaveService
    {
        private readonly IConfiguration _config;

        public LeaveService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> GetBalanceAsync(int userId)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var balance = await connection.QuerySingleAsync<int>(
                "SELECT Balance FROM Leave WHERE UserId = @UserId",
                new { UserId = userId });

            return balance;
        }
    }
}