using log4net;

namespace deployd.Features
{
    public interface ILoggingConfiguration
    {
        void SetLogLevelToDebug();
    }

    public class LoggingConfiguration : ILoggingConfiguration
    {
        private readonly ILog _log;

        public LoggingConfiguration(ILog log)
        {
            _log = log;
        }

        public void SetLogLevelToDebug()
        {
            SetLogLevel("DEBUG");
        }

        private void SetLogLevel(string level)
        {
            var heir =
                (log4net.Repository.Hierarchy.Hierarchy)
                (((log4net.Repository.Hierarchy.Logger) _log.Logger).Repository);

            ((log4net.Repository.Hierarchy.Logger)_log.Logger).Level = heir.LevelMap[level];
        }
    }
}
