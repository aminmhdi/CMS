using System;
using System.Text.Json;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Identity;
using CMS.ViewModel.Logger;
using CMS.ViewModel.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace CMS.ServiceLayer.Logger
{
    public class DbLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly IServiceProvider _serviceProvider;
        private readonly DbLoggerProvider _loggerProvider;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly LogLevel _minLevel;

        public DbLogger
        (
            DbLoggerProvider loggerProvider,
            IServiceProvider serviceProvider,
            string loggerName,
            IOptions<AppSettings> appSettings
            )
        {
            _loggerName = loggerName;
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(_appSettings));
            _minLevel = _appSettings.Value.Logging.LogLevel.Default;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider));
            _loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(_loggerProvider));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLevel;
        }

        public void Log<TState>
        (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (exception != null)
            {
                message = $"{message}{Environment.NewLine}{exception}";
            }

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var appLogItem = new AppLogItem
            {
                Url = httpContextAccessor?.HttpContext != null
                    ? httpContextAccessor.HttpContext.Request.Path.ToString()
                    : string.Empty,
                EventId = eventId.Id,
                LogLevel = logLevel.ToString(),
                Logger = _loggerName,
                Message = message
            };
            var props = httpContextAccessor?.GetShadowProperties();
            SetStateJson(state, appLogItem);
            _loggerProvider.AddLogItem(new LoggerItem {Props = props, AppLogItem = appLogItem});
        }

        private static void SetStateJson<TState>(TState state, AppLogItem appLogItem)
        {
            try
            {
                appLogItem.StateJson = JsonSerializer.Serialize(
                    state,
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        WriteIndented = true
                    });
            }
            catch
            {
                // don't throw exceptions from logger
            }
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}