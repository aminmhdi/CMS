using ORS.ViewModels.Identity.Settings;

namespace CMS.ViewModel.Settings
{
    public class ConnectionStrings
    {
        public SqlServer SqlServer { get; set; }
        public Localdb LocalDb { get; set; }
        public SQLite SqLite { get; set; }
    }
}
