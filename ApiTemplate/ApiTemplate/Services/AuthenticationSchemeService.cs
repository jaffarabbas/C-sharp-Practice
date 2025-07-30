using ApiTemplate.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApiTemplate.Services
{
    public static class AuthenticationSchemeService
    {
        /// <summary>
        /// Adds Windows Authentication if enabled in configuration, otherwise adds default Cookie Authentication.
        /// </summary>
        public static IServiceCollection AddAuthenticationSchemeService(this IServiceCollection services, IConfiguration configuration)
        {
            // Read the setting from configuration
            bool enableWindowsAuth = configuration.GetValue<bool>("Authentication:EnableWindowsAuthentication");

            if (enableWindowsAuth)  
            {
                services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
            }
            else
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = "/api/auth/login";
                            options.LogoutPath = "/api/auth/logout";
                            options.AccessDeniedPath = "/api/auth/accessdenied";
                            options.Cookie.Name = "ApiTemplateAuth";
                            options.Cookie.HttpOnly = true;
                            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                        });
            }

            return services;
        }
    }
}
