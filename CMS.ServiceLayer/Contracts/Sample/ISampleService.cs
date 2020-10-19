using System.Threading.Tasks;
using CMS.ViewModel.Sample;

namespace CMS.ServiceLayer.Contracts.Sample
{
    public interface ISampleService
    {
        Task<SampleListViewModel> List(SampleSearchViewModel search);
        Task<int> Create(SampleViewModel viewModel);
        Task<int> Edit(SampleViewModel viewModel);
        Task<SampleViewModel> Get(int id);
        Task<bool> Exists();
        Task<bool> Exists(int id);
        Task<int> Delete(int id);
    }
}
