using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Common.DateToolkit;
using CMS.Common.GuardToolkit;
using CMS.DataLayer.Context;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Common.Enums;
using CMS.ServiceLayer.Contracts.Identity;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ViewModel.Sample;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMS.ServiceLayer.Sample
{
    public class SampleService : ISampleService
    {
        #region Constructor

        private readonly DbSet<Entities.Sample.Sample> _sample;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _contextAccessor;

        public SampleService
        (
            IUnitOfWork uow,
            IHttpContextAccessor contextAccessor
        )
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _sample = _uow.Set<Entities.Sample.Sample>();

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(contextAccessor));
        }

        #endregion

        #region List

        public async Task<SampleListViewModel> List(SampleSearchViewModel search)
        {
            var query = _sample.AsNoTracking().AsQueryable();

            query = query.Where(q => q.Status == Status.Active).AsQueryable();

            if (search.Id.HasValue)
                query = query.Where(q => q.Id == search.Id.Value).AsQueryable();

            if (!string.IsNullOrEmpty(search.Title))
                query = query.Where(q => q.Title.Contains(search.Title)).AsQueryable();

            if (search.CreateFrom.HasValue)
            {
                var createFromDateTime = search.CreateFrom.ToDateTimeFromUnix();
                query = query.Where(q => EF.Property<DateTime>(q, AuditableShadowProperties.CreatedDateTime) >= createFromDateTime).AsQueryable();
            }


            if (search.CreateTo.HasValue)
            {
                var createToDateTime = search.CreateFrom.ToDateTimeFromUnix();
                query = query.Where(q => EF.Property<DateTime>(q, AuditableShadowProperties.CreatedDateTime) <= createToDateTime).AsQueryable();
            }

            query = search.OrderBy switch
            {
                "Id" => search.Order == SortOrder.Descending
                    ? query.OrderByDescending(q => q.Id).AsQueryable()
                    : query.OrderBy(q => q.Id).AsQueryable(),
                "CreateDate" => search.Order == SortOrder.Descending
                    ? query.OrderByDescending(q => EF.Property<DateTime>(q, AuditableShadowProperties.CreatedDateTime)).AsQueryable()
                    : query.OrderBy(q => EF.Property<DateTime>(q, AuditableShadowProperties.CreatedDateTime)).AsQueryable(),
                _ => query.OrderByDescending(x => x.Id)
            };

            var offset = (search.PageNumber - 1) * search.PageSize;

            search.Total = query.Count();
            search.TotalPage = (int) Math.Ceiling((decimal) search.Total / search.PageSize);


            var list = await query
                .Skip(offset)
                .Take(search.PageSize)
                .ToListAsync();

            var viewModel = new SampleListViewModel
            {
                Search = search,
                List = list.Select(q => new SampleViewModel
                {
                    Id = q.Id,
                    Title = q.Title
                }).ToList()
            };

            viewModel.Search.Total = search.Total;
            viewModel.Search.TotalPage = search.TotalPage;
            viewModel.Search.PageNumber = search.PageNumber;
            viewModel.Search.PageSize = search.PageSize;

            return viewModel;
        }

        #endregion

        #region Create

        public async Task<int> Create(SampleViewModel viewModel)
        {
            if (viewModel == null)
                return 0;

            var model = new Entities.Sample.Sample
            {
                Title = viewModel.Title,
                Status = Status.Active
            };

            await _sample.AddAsync(model);
            return await _uow.SaveChangesAsync();
        }

        #endregion

        #region Edit

        public async Task<int> Edit(SampleViewModel viewModel)
        {
            if (viewModel == null)
                return 0;

            var model = await _sample.FirstOrDefaultAsync(q => q.Id == viewModel.Id);

            if (model == null)
                return 0;

            model.Id = viewModel.Id;
            model.Title = viewModel.Title;

            _sample.Update(model);

            return await _uow.SaveChangesAsync();
        }

        #endregion

        #region Get

        public async Task<SampleViewModel> Get(int id)
        {
            var model = await _sample.FirstOrDefaultAsync(p => p.Id == id);

            var viewModel = new SampleViewModel
            {
                Id = model.Id,
                Title = model.Title
            };

            return viewModel;
        }

        public async Task<bool> Exists(int id)
        {
            return await _sample.AnyAsync(e => e.Id == id);
        }

        #endregion

        #region Delete

        public async Task<int> Delete(int id)
        {
            // Physical Delete
            //var receipt = await _sample.FindAsync(id);
            //if (receipt == null)
            //    return 0;
            //_sample.Remove(receipt);
            //return await _uow.SaveChangesAsync();

            // Logical Delete
            var receipt = await _sample.FindAsync(id);
            if (receipt == null)
                return 0;
            receipt.Status = Status.Deleted;
            _sample.Update(receipt);
            return await _uow.SaveChangesAsync();
        }

        #endregion
    }
}
