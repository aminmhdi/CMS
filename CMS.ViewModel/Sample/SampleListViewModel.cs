using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.ViewModel.Sample
{
    public class SampleListViewModel
    {
        public SampleSearchViewModel Search { get; set; }
        public List<SampleViewModel> List { get; set; }
    }
}
