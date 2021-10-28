using APIFrame.Core.Models;
using APIFrame.DataAccess.Repositories.Interfaces;
using APIFrame.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace APIFrame.Web.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _fileLogger;
        private readonly ILogRepo _logRepo;

        public LogService(ILogger<LogService> fileLogger, ILogRepo logRepo)
        {
            _fileLogger = fileLogger;
            _logRepo = logRepo;
        }

        public async Task LogAsync(Log log)
        {
            var fileLogContent = $"({log.LogType}) {log.Message}";
            switch (log.LogLevel)
            {
                case Core.Enums.LogLevel.Debug:
                    _fileLogger.LogDebug(fileLogContent);
                    break;
                case Core.Enums.LogLevel.Info:
                    _fileLogger.LogInformation(fileLogContent);
                    break;
                case Core.Enums.LogLevel.Warn:
                    _fileLogger.LogWarning(fileLogContent);
                    break;
                case Core.Enums.LogLevel.Error:
                    _fileLogger.LogError(fileLogContent);
                    break;
                case Core.Enums.LogLevel.Critical:
                    _fileLogger.LogCritical(fileLogContent);
                    break;
                default:
                    _fileLogger.LogInformation(fileLogContent);
                    break;
            }

            log.CreationDate = DateTime.Now;
            log.IsActive = true;

            await _logRepo.AddAsync(log);
        }
    }
}
