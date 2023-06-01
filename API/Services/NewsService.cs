using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;

namespace API.Services
{
    public interface INewsService : IBaseService
    {
        Task<ApiResponses<NewsDto>> GetNews(NewsQueryDto queryDto);
        Task<ApiResponse<bool>> InsertNews(NewsCreateDto newsDto);
        Task<ApiResponse<bool>> UpdateNews(Guid Id, NewsUpdateDto newsDto);
        Task<ApiResponse<bool>> DeleteNews(NewsDeleteDto newsDto);
    }

    public class NewsService : BaseService, INewsService
    {
        public NewsService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
        {
        }

        public async Task<ApiResponse<bool>> DeleteNews(NewsDeleteDto newsDto)
        {
            var existingNews= await MainUnitOfWork.NewsRepository.FindOneAsync(newsDto.Id);
            if (existingNews == null)
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
            }

            bool isDeleted = await MainUnitOfWork.NewsRepository.DeleteAsync(existingNews, AccountId);

            if (isDeleted)
            {
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
            }
        }

        public async Task<ApiResponses<NewsDto>> GetNews(NewsQueryDto queryDto)
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

        public async Task<ApiResponse<bool>> InsertNews(NewsCreateDto newsDto)
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
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return (ApiResponse<bool>)ApiResponse.Failed();
            }
        }

        public async Task<ApiResponse<bool>> UpdateNews(Guid Id, NewsUpdateDto newsDto)
        {
            var existingNews = await MainUnitOfWork.NewsRepository.FindOneAsync(Id);

            if (existingNews == null)
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
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
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
            }
        }
    }
}
