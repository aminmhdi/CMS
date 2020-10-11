using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace CMS.ViewModel.Settings
{
    public class LogLevel
    {
        public MsLogLevel Default { get; set; }
        public MsLogLevel System { get; set; }
        public MsLogLevel Microsoft { get; set; }
    }
}
