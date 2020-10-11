using CMS.Entities.AuditableEntity;
using CMS.Entities.Identity;

namespace CMS.ViewModel.Logger
{
    public class LoggerItem
    {
        public AppShadowProperties Props { set; get; }
        public AppLogItem AppLogItem { set; get; }
    }
}
