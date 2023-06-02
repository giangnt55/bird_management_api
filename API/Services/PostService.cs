using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;

namespace API.Services
{
    public interface IPostService : IBaseService 
    {
        Task<ApiResponses<Post>> GetPosts();
        Task<ApiResponse<bool>> InsertPost(PostCreateDto postDto);
        Task<ApiResponse<bool>> DeletePost(PostDeleteDto postDto);
    }

    public class PostService : BaseService, IPostService
    {
        public PostService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
        {
        }

        public async Task<ApiResponse<bool>> DeletePost(PostDeleteDto postDto)
        {
            var existingPost = await MainUnitOfWork.PostRepository.FindOneAsync(postDto.Id);
            if (existingPost != null)
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
            }

            bool isDeleted = await MainUnitOfWork.PostRepository.DeleteAsync(existingPost, AccountId);

            if (isDeleted)
            {
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return (ApiResponse<bool>)ApiResponse<bool>.Failed();
            }
        }

        public async Task<ApiResponses<Post>> GetPosts()
        {
            string orderBy = "CreatedAt desc";
            var response = await MainUnitOfWork.PostRepository.FindAsync(null, orderBy);

            return ApiResponses<Post>.Success(response);
        }

        public async Task<ApiResponse<bool>> InsertPost(PostCreateDto postDto)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                Tittle = postDto.Tittle,
                Content = postDto.Content,
                Image = postDto.Image
            };

            bool response = await MainUnitOfWork.PostRepository.InsertAsync(post, AccountId);

            if (response)
            {
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return (ApiResponse<bool>)ApiResponse.Failed();
            }
        }
    }
}
