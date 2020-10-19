namespace CMS.ViewModel.Settings
{
    public class AppSettings
    {
        public Logging Logging { get; set; }

        public SampleSeed SampleSeed { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public ActiveDatabase ActiveDatabase { get; set; }
    }
}
