using DeploymentTool.Models;
using Microsoft.Extensions.Configuration;

namespace DeploymentTool.Services;

public class ConfigService
{
    public AppSettings Settings { get; }

    public ConfigService()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        Settings = config.Get<AppSettings>() ?? new AppSettings();
    }
}
