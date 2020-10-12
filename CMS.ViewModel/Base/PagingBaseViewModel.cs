using Microsoft.Data.SqlClient;

namespace CMS.ViewModel.Base
{
    public class PagingBaseViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortOrder Order { get; set; } = SortOrder.Ascending;
        public string OrderBy { get; set; } = "Id";

        public int TotalPage { get; set; }
        public int Total { get; set; }
    }
}
