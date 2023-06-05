using API.Dtos;
using API.Services;
using AppCore.Models;
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
        public async Task<ApiResponses<NewsDto>> GetAllNews([FromQuery] NewsQueryDto queryDto)
        {
            return await _service.GetAllNews(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Insert ...")]
        public async Task<ApiResponse> InsertNews(NewsCreateDto newsDto)
        {
            return await _service.InsertNews(newsDto);
        }

        [HttpPut]
        [SwaggerOperation("Update ...")]
        public async Task<ApiResponse> UpdateNews(Guid Id, NewsUpdateDto newsDto)
        {
            return await _service.UpdateNews(Id, newsDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete ...")]
        public async Task<ApiResponse> DeleteNews(GetNewsDto newsDto)
        {
            return await _service.DeleteNews(newsDto);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Detail ...")]
        public async Task<ApiResponse> GetNewsDetail(Guid id)
        {
            return await _service.GetNewsDetail(id);
        }
    }
}
