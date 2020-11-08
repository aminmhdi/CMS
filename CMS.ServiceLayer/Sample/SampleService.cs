using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Common.DateToolkit;
using CMS.Common.GuardToolkit;
using CMS.DataLayer.Context;
using CMS.Entities.Common.Enums;
using CMS.Entities.Sample;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ViewModel.Sample;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;

namespace CMS.ServiceLayer.Sample
{
    public class SampleService : ISampleService
    {
        #region Constructor

        private readonly IMongoDbContext _mongoDbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public SampleService
        (
            IMongoDbContext mongoDbContext,
            IHttpContextAccessor contextAccessor
        )
        {
            _mongoDbContext = mongoDbContext;
            _mongoDbContext.CheckArgumentIsNull(nameof(_mongoDbContext));

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(contextAccessor));
        }

        #endregion

        #region List

        public async Task<SampleListViewModel> List(SampleSearchViewModel search)
        {
            var filter = Builders<SampleModel>.Filter.Where(q => q.Status == Status.Active);

            if (search.Id.HasValue)
                filter &= Builders<SampleModel>.Filter.Where(q => q.Id == search.Id.Value);

            if (!string.IsNullOrEmpty(search.Title))
                filter &= Builders<SampleModel>.Filter.Where(q => q.Title.Contains(search.Title));

            if (search.CreateFrom.HasValue)
            {
                var createFromDateTime = search.CreateFrom.ToDateTimeFromUnix();
                filter &= Builders<SampleModel>.Filter.Where(q => q.CreateDate >= createFromDateTime);
            }

            if (search.CreateTo.HasValue)
            {
                var createToDateTime = search.CreateFrom.ToDateTimeFromUnix();
                filter &= Builders<SampleModel>.Filter.Where(q => q.CreateDate <= createToDateTime);
            }

            var sort = search.OrderBy switch
            {
                "Id" => search.Order == SortOrder.Descending
                    ? Builders<SampleModel>.Sort.Descending(q => q.Id)
                    : Builders<SampleModel>.Sort.Ascending(q => q.Id),
                "CreateDate" => search.Order == SortOrder.Descending
                    ? Builders<SampleModel>.Sort.Descending(q => q.CreateDate)
                    : Builders<SampleModel>.Sort.Ascending(q => q.CreateDate),
                _ => Builders<SampleModel>.Sort.Descending(x => x.Id)
            };

            var offset = (search.PageNumber - 1) * search.PageSize;

            var list = await _mongoDbContext.Sample
                .Find(filter)
                .Sort(sort)
                .Skip(offset)
                .Limit(search.PageSize)
                .ToListAsync();


            search.Total = await _mongoDbContext.Sample.Find(filter).CountDocumentsAsync();
            search.TotalPage = (int)Math.Ceiling((decimal)search.Total / search.PageSize);


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

            var model = new SampleModel
            {
                Title = viewModel.Title,
                CreateDate = DateTime.Now,
                Status = Status.Active
            };

            await _mongoDbContext.Sample.InsertOneAsync(model);
            return 1;
        }

        #endregion

        #region Edit

        public async Task<int> Edit(SampleViewModel viewModel)
        {
            if (viewModel == null)
                return 0;

            var model = (await _mongoDbContext.Sample.FindAsync(q => q.Id == viewModel.Id)).FirstOrDefault();

            if (model == null)
                return 0;

            model.Id = viewModel.Id;
            model.Title = viewModel.Title;
            model.UpdateDate = DateTime.Now;

            var updateFilter = Builders<SampleModel>.Filter.Eq(q => q.Id, model.Id);
            var updateTitle = Builders<SampleModel>.Update.Set(q => q.Title, model.Title);
            var updateDate = updateTitle.Set(q => q.UpdateDate, model.UpdateDate);

            await _mongoDbContext.Sample.UpdateOneAsync(updateFilter, updateDate);

            return model.Id;
        }

        #endregion

        #region Get

        public async Task<SampleViewModel> Get(int id)
        {
            var model = (await _mongoDbContext.Sample.FindAsync(q => q.Id == id)).FirstOrDefault();

            if (model == null)
                return null;

            var viewModel = new SampleViewModel
            {
                Id = model.Id,
                Title = model.Title
            };

            return viewModel;
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
            var receipt = (await _mongoDbContext.Sample.FindAsync(q => q.Id == id)).FirstOrDefault();
            if (receipt == null)
                return 0;

            receipt.Status = Status.Deleted;
            receipt.UpdateDate = DateTime.Now;

            var updateFilter = Builders<SampleModel>.Filter.Eq(q => q.Id, receipt.Id);
            var updateStatus = Builders<SampleModel>.Update.Set(q => q.Status, receipt.Status);
            var updateDate = updateStatus.Set(q => q.UpdateDate, receipt.UpdateDate);

            await _mongoDbContext.Sample.UpdateOneAsync(updateFilter, updateDate);

            return receipt.Id;
        }

        #endregion

        #region Exists

        public async Task<bool> Exists()
        {
            return await _mongoDbContext.Sample.CountDocumentsAsync(FilterDefinition<SampleModel>.Empty) > 0;
        }

        public async Task<bool> Exists(int id)
        {
            return (await _mongoDbContext.Sample.FindAsync(q => q.Id == id)).FirstOrDefault() != null;
        }

        #endregion
    }
}
