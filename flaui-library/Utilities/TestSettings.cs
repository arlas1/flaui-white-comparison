using Microsoft.Extensions.Configuration;

namespace flaui_library.Utilities;

public class ApplicationSettings
{
    public string ProcessName { get; set; } = "TestProcess.exe";
    public string LaunchPath { get; set; } = "";
    public string LaunchArguments { get; set; } = "";
}

public class TimeoutSettings
{
    public int DefaultTimeoutSeconds { get; set; } = 30;
    public int RetryIntervalMs { get; set; } = 200;
    public int WindowAttachTimeoutMinutes { get; set; } = 1;
    public TimeSpan DefaultTimeout => TimeSpan.FromSeconds(DefaultTimeoutSeconds);
    public TimeSpan RetryInterval => TimeSpan.FromMilliseconds(RetryIntervalMs);
    public TimeSpan WindowAttachTimeout => TimeSpan.FromMinutes(WindowAttachTimeoutMinutes);
}

public static class TestSettings
{
    private static readonly Lazy<IConfigurationRoot> _config = new(() =>
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("Config\\appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();
    });

    private static ApplicationSettings _app;
    private static TimeoutSettings _timeouts;

    public static ApplicationSettings Application
    {
        get
        {
            if (_app == null)
            {
                _app = new ApplicationSettings();
                _config.Value.GetSection("Application").Bind(_app);
            }

            return _app;
        }
    }

    public static TimeoutSettings Timeouts
    {
        get
        {
            if (_timeouts == null)
            {
                _timeouts = new TimeoutSettings();
                _config.Value.GetSection("Timeouts").Bind(_timeouts);
            }

            return _timeouts;
        }
    }
}
