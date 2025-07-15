using ApiTemplate.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiTemplate.Services
{
    public static class AddJWTService
    {
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            var _jwtSettng = configuration.GetSection("JWTSetting");
            var authkey = configuration.GetValue<string>("JWTSetting:securitykey");

            services.Configure<JWTSetting>(_jwtSettng);
            services.AddAuthentication(item =>
             {
                 item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             }).AddJwtBearer(options =>
             {
                 options.RequireHttpsMetadata = true;
                 options.SaveToken = true;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey.ToString())),
                     ValidateIssuer = false,
                     ValidateAudience = false
                 };
             });
            return services;
        }
    }
}
