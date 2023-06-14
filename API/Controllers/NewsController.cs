using API.Dtos;
using API.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    public class NewsController : BaseController
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list ...")]
        public async Task<ApiResponses<NewsDto>> GetListNews([FromQuery] NewsQueryDto queryDto)
        {
            return await _service.GetNews(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Insert ...")]
        public async Task<ApiResponse<bool>> InsertNews(NewsCreateDto newsDto)
        {
            return await _service.InsertNews(newsDto);
        }

        [HttpPut]
        [SwaggerOperation("Update ...")]
        public async Task<ApiResponse<bool>> UpdateNews(Guid Id, NewsUpdateDto newsDto)
        {
            return await _service.UpdateNews(Id, newsDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete ...")]
        public async Task<ApiResponse<bool>> DeleteNews(NewsDeleteDto newsDto)
        {
            return await _service.DeleteNews(newsDto);
        }
    }
}
