using CMS.ViewModel.Base;

namespace CMS.ViewModel.Sample
{
    public class SampleSearchViewModel : PagingBaseViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }

        public long? CreateFrom { get; set; }
        public long? CreateTo { get; set; }
    }
}
