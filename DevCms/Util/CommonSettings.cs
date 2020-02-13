using Microsoft.Extensions.Configuration;

namespace DevCms.Util
{
    public static class CommonSettings
    {
        public static int FailedLoginAttemptLimit
        {
            get
            {
                return Config.GetValue<int>("FailedLoginAttemptLimit");
            }
        }

        public static int AttemptMinute
        {
            get
            {
                return Config.GetValue<int>("AttemptMinute");
            }
        }

        private static IConfiguration _config;
        private static IConfiguration Config
        {
            get
            {
                if (_config != null)
                    return _config;
                var builder = new ConfigurationBuilder()
                    .SetBasePath(System.AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _config = builder.Build().GetSection("AppConfiguration");
                return _config;
            }
        }
    }

    public class ConfigHelper
    {
        public static IConfiguration GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
