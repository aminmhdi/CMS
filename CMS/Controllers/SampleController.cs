using System;
using System.Threading.Tasks;
using CMS.Common.GuardToolkit;
using CMS.ServiceLayer.Contracts.Resources;
using CMS.ServiceLayer.Contracts.Sample;
using CMS.ViewModel.Sample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Controllers
{
    public class SampleController : BaseController
    {
        private readonly ILogger<SampleController> _logger;
        private readonly ISampleService _sampleService;

        public SampleController
        (
            ISharedResource sharedResource,
            ILogger<SampleController> logger,
            ISampleService sampleService
        ) : base(sharedResource)
        {
            _logger = logger;
            _sampleService = sampleService;
            _sampleService.CheckArgumentIsNull(nameof(_sampleService));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> List(SampleSearchViewModel search)
        {
            try
            {
                var viewModel = await _sampleService.List(search);
                return Ok(new { SampleList = viewModel.List, viewModel.Search.TotalPage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create(SampleViewModel viewModel)
        {
            try
            {
                var result = await _sampleService.Create(viewModel);
                if (result > 0)
                    return BaseOk();
                return BaseBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Edit(SampleViewModel viewModel)
        {
            try
            {
                var result = await _sampleService.Edit(viewModel);
                if (result > 0)
                    return Ok();
                return BaseBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _sampleService.Delete(id);
                if (result > 0)
                    return BaseOk();
                return BaseBadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
