using Serilog;
using Serilog.Core;

namespace Web_UI_Automation.Core
{
    public static class LoggerManager
    {
        private static ILogger _logger;

        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.File("logs/test-execution-.log",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                        .CreateLogger();
                }
                return _logger;
            }
        }

        public static void CloseLogger()
        {
            Log.CloseAndFlush();
        }
    }
}