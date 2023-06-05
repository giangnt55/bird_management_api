using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;

namespace API.Services
{
    public interface INewsService : IBaseService
    {
        Task<ApiResponses<NewsDto>> GetAllNews(NewsQueryDto queryDto);
        Task<ApiResponse<News>> InsertNews(NewsCreateDto newsDto);
        Task<ApiResponse<News>> UpdateNews(Guid Id, NewsUpdateDto newsDto);
        Task<ApiResponse> DeleteNews(GetNewsDto newsDto);

        Task<ApiResponse<News>> GetNewsDetail(Guid id);
    }

    public class NewsService : BaseService, INewsService
    {
        public NewsService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
        {
        }

        public async Task<ApiResponse> DeleteNews(GetNewsDto newsDto)
        {
            var existingNews= await MainUnitOfWork.NewsRepository.FindOneAsync(newsDto.Id);
            if (existingNews == null)
            {
                return ApiResponse.Failed();
            }

            bool isDeleted = await MainUnitOfWork.NewsRepository.DeleteAsync(existingNews, AccountId);

            if (isDeleted)
            {
                return ApiResponse.Success("Delete successfully");
            }
            else
            {
                return ApiResponse.Failed();
            }
        }

        public async Task<ApiResponses<NewsDto>> GetAllNews(NewsQueryDto queryDto)
        {
            var response = await MainUnitOfWork.NewsRepository.FindResultAsync<NewsDto>(new Expression<Func<News, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.Title) || x.Title.ToLower() == queryDto.Title.Trim().ToLower()
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            return ApiResponses<NewsDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<News>> GetNewsDetail(Guid id)
        {
            var response = await MainUnitOfWork.NewsRepository.FindOneAsync(id);

            return ApiResponse<News>.Success(response);
        }

        public async Task<ApiResponse<News>> InsertNews(NewsCreateDto newsDto)
        {
            var news = new News
            {
                Id = Guid.NewGuid(),
                Title = newsDto.Title,
                Type = newsDto.Type,
                Content = newsDto.Content,
                CoverImage = newsDto.CoverImage,
                PublishDate = newsDto.PublishDate,
                Author = newsDto.Author,
            };
            bool response = await MainUnitOfWork.NewsRepository.InsertAsync(news, AccountId);

            if (response)
            {
                return ApiResponse<News>.Success(news);
            }
            else
            {
                return (ApiResponse<News>)ApiResponse.Failed();
            }
        }

        public async Task<ApiResponse<News>> UpdateNews(Guid Id, NewsUpdateDto newsDto)
        {
            var existingNews = await MainUnitOfWork.NewsRepository.FindOneAsync(Id);

            if (existingNews == null)
            {
                return (ApiResponse<News>)ApiResponse.Failed();
            }

            var news = existingNews;

            existingNews.Title = newsDto.Title ?? existingNews.Title;
            existingNews.Type = newsDto.Type;
            existingNews.Content = newsDto.Content ?? existingNews.Content;
            existingNews.CoverImage = newsDto.CoverImage ?? existingNews.CoverImage;
            existingNews.PublishDate = newsDto.PublishDate;
            existingNews.Author = newsDto.Author ?? existingNews.Author;

            bool isUpdated = await MainUnitOfWork.NewsRepository.UpdateAsync(news, AccountId);

            if (isUpdated)
            {
                return ApiResponse<News>.Success(news);
            }
            else
            {
                return (ApiResponse<News>)ApiResponse.Failed();
            }
        }
    }
}
