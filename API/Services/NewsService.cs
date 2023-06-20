using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;
using AppCore.Extensions;
using MainData.Repositories;

namespace API.Services
{
    public interface INewsService : IBaseService
    {
        Task<ApiResponses<NewsDto>> GetAllNews(NewsQueryDto queryDto);
        Task<ApiResponse<NewsDto>> InsertNews(NewsCreateDto newsDto);
        Task<ApiResponse<NewsDto>> UpdateNews(Guid Id, NewsUpdateDto newsDto);
        Task<ApiResponse> DeleteNews(GetNewsDto newsDto);

        Task<ApiResponse<NewsDto>> GetNewsDetail(Guid id);
    }

    public class NewsService : BaseService, INewsService
    {
        public NewsService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> DeleteNews(GetNewsDto newsDto)
        {
            var existingNews= await MainUnitOfWork.NewsRepository.FindOneAsync(newsDto.Id);
            if (existingNews == null)
                throw new ApiException("Not found this news", StatusCode.NOT_FOUND);

            if (existingNews.CreatorId != AccountId)
                throw new ApiException("Can't not delete others news", StatusCode.BAD_REQUEST);

            if (await MainUnitOfWork.NewsRepository.DeleteAsync(existingNews, AccountId))
                throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);

            return ApiResponse.Success();
        }

        public async Task<ApiResponses<NewsDto>> GetAllNews(NewsQueryDto queryDto)
        {
            var response = await MainUnitOfWork.NewsRepository.FindResultAsync<NewsDto>(new Expression<Func<News, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.Title) || x.Title.ToLower() == queryDto.Title.Trim().ToLower()
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            response.Items = await _mapperRepository.MapCreator(response.Items.ToList());

            return ApiResponses<NewsDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<NewsDto>> GetNewsDetail(Guid id)
        {
          var response = await MainUnitOfWork.NewsRepository.FindOneAsync<NewsDto>(new Expression<Func<News, bool>>[]
          {
            x => !x.DeletedAt.HasValue,
            x => x.Id == id
          });

          if (response == null)
            throw new ApiException("Not found this news", StatusCode.NOT_FOUND);

          response = await _mapperRepository.MapCreator(response);
            return ApiResponse<NewsDto>.Success(response);
        }

        public async Task<ApiResponse<NewsDto>> InsertNews(NewsCreateDto newsDto)
        {
            var news = newsDto.ProjectTo<NewsCreateDto, News>();
            news.Id = Guid.NewGuid();
            if (!await MainUnitOfWork.NewsRepository.InsertAsync(news, AccountId))
              throw new ApiException("Can't insert", StatusCode.SERVER_ERROR);

            return await GetNewsDetail(news.Id);
        }

        public async Task<ApiResponse<NewsDto>> UpdateNews(Guid id, NewsUpdateDto newsDto)
        {
            var existingNews = await MainUnitOfWork.NewsRepository.FindOneAsync(id);

            if (existingNews == null)
              throw new ApiException("Not found this news", StatusCode.NOT_FOUND);

            existingNews.Title = newsDto.Title ?? existingNews.Title;
            existingNews.Type = newsDto.Type ?? existingNews.Type;
            existingNews.Content = newsDto.Content ?? existingNews.Content;
            existingNews.CoverImage = newsDto.CoverImage ?? existingNews.CoverImage;

            if (! await MainUnitOfWork.NewsRepository.UpdateAsync(existingNews, AccountId))
              throw new ApiException("Can't update", StatusCode.SERVER_ERROR);

            return await GetNewsDetail(existingNews.Id);
        }

    }
}
