using APIFrame.Core.Enums;

namespace APIFrame.Core.Models
{
    public class Log : BaseEntity
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        public LogType LogType { get; set; } = LogType.General;

        public string Executor { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
