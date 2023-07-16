// using API.Dtos;
// using API.Services;
// using AppCore.Models;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace API.Controllers
// {
//     public class NewsController : BaseController
//     {
//         private readonly INewsService _service;
//
//         public NewsController(INewsService service)
//         {
//             _service = service;
//         }
//
//         [HttpGet]
//         [SwaggerOperation("Get list news")]
//         public async Task<ApiResponses<NewsDto>> GetAllNews([FromQuery] NewsQueryDto queryDto)
//         {
//             return await _service.GetAllNews(queryDto);
//         }
//
//         [HttpPost]
//         [SwaggerOperation("Insert news")]
//         public async Task<ApiResponse<NewsDto>> InsertNews(NewsCreateDto newsDto)
//         {
//             return await _service.InsertNews(newsDto);
//         }
//
//         [HttpPut]
//         [SwaggerOperation("Update news")]
//         public async Task<ApiResponse<NewsDto>> UpdateNews(Guid id, NewsUpdateDto newsDto)
//         {
//             return await _service.UpdateNews(id, newsDto);
//         }
//
//         [HttpDelete]
//         [SwaggerOperation("Delete news")]
//         public async Task<ApiResponse> DeleteNews(GetNewsDto newsDto)
//         {
//             return await _service.DeleteNews(newsDto);
//         }
//
//         [HttpGet("{id}")]
//         [SwaggerOperation("Detail news")]
//         public async Task<ApiResponse<NewsDto>> GetNewsDetail(Guid id)
//         {
//             return await _service.GetNewsDetail(id);
//         }
//     }
// }
