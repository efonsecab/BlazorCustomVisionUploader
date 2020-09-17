using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorCustomVisionUploader.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;

namespace BlazorCustomVisionUploader.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomVisionUploaderController : ControllerBase
    {
        public ILogger<CustomVisionUploaderController> Logger { get; }
        private AzureBingSearchService AzureBingSearchService { get; }
        private AzureCustomVisionService AzureCustomVisionService { get; }

        public CustomVisionUploaderController(ILogger<CustomVisionUploaderController> logger,
            AzureBingSearchService azureBingSearchService, 
            AzureCustomVisionService azureCustomVisionService)
        {
            this.Logger = logger;
            this.AzureBingSearchService = azureBingSearchService;
            this.AzureCustomVisionService = azureCustomVisionService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var searchResponse = await this.AzureBingSearchService.SearchImagesAsync(searchTerm,
                AzureBingSearchService.SafeSearchMode.Strict);
            var result = searchResponse.value.Select(p => new SearchResultItem()
            {
                ImageUrl = p.contentUrl
            }).ToList();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImages([FromBody]UploadImagesModel model)
        {
            //Guid projectId = Guid.Parse("3a7bfe46-2fa2-4304-a4a7-b4bf54ee9ac3");
            Guid projectId = Guid.Parse(model.ProjectId);
            List<Uri> lstImages = model.Items.Where(p=>p.IsSelected==true).Select(p => new Uri(p.ImageUrl)).ToList();
            var uploadImagesResult = await AzureCustomVisionService.UploadImagesAsync(lstImages, projectId);
            List<string> tags = new List<string>() { model.Tag };
            foreach (var singleImage in uploadImagesResult)
            {
                try
                {
                    await this.AzureCustomVisionService.CreateImageTagsAsync(projectId,
                        singleImage.Image.Id,
                        tags);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, ex.Message);
                }
            }
            return Ok();
        }
    }
}
