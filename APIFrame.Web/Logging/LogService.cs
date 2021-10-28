using APIFrame.Core.Models;
using APIFrame.DataAccess.Repositories.Interfaces;
using APIFrame.Web.Authentication.Interfaces;
using APIFrame.Web.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace APIFrame.Logging.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _fileLogger;
        private readonly ILogRepo _logRepo;
        private IContextInfo _contextInfo;

        public LogService(
            ILogger<LogService> fileLogger, 
            ILogRepo logRepo,
            IContextInfo contextInfo)
        {
            _fileLogger = fileLogger;
            _logRepo = logRepo;
            _contextInfo = contextInfo;
        }

        public async Task LogAsync(Log log)
        {
            var fileLogContentSb = new StringBuilder($"({log.LogType}) {log.Message}");

            if (!string.IsNullOrEmpty(_contextInfo.AuthToken) || !string.IsNullOrEmpty(_contextInfo.UserId))
            {
                fileLogContentSb.Append(";\nContextInfo: {_authContextInfo}");
            }

            switch (log.LogLevel)
            {
                case Core.Enums.LogLevel.Debug:
                    _fileLogger.LogDebug(fileLogContentSb.ToString());
                    break;
                case Core.Enums.LogLevel.Info:
                    _fileLogger.LogInformation(fileLogContentSb.ToString());
                    break;
                case Core.Enums.LogLevel.Warn:
                    _fileLogger.LogWarning(fileLogContentSb.ToString());
                    break;
                case Core.Enums.LogLevel.Error:
                    _fileLogger.LogError(fileLogContentSb.ToString());
                    break;
                case Core.Enums.LogLevel.Critical:
                    _fileLogger.LogCritical(fileLogContentSb.ToString());
                    break;
                default:
                    _fileLogger.LogInformation(fileLogContentSb.ToString());
                    break;
            }

            if (string.IsNullOrEmpty(log.Executor))
            {
                log.Executor = _contextInfo.UserId;
            }

            if (log.CreationDate == default)
            {
                log.CreationDate = DateTime.Now;
            }


            await _logRepo.AddAsync(log);
        }
    }
}
