using System.Data;
using DBLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBLayer
{
    /// <summary>
    /// Centralized database registration (EF Core + Dapper connection).
    /// </summary>
    public static class DbContextDI
    {
        /// <summary>
        /// Registers TestContext and an IDbConnection (SqlConnection) using the "DefaultConnection" string.
        /// </summary>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<TestContext>(options =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    // Point migrations to the DBLayer assembly
                    sql.MigrationsAssembly(typeof(TestContext).Assembly.FullName);
                });

                // Keep your current no-tracking default
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            // Dapper connection (scoped per request)
            services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

            return services;
        }
    }
}