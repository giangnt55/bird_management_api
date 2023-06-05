using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;

namespace API.Services
{
    public interface IPostService : IBaseService
    {
        Task<ApiResponses<PostDto>> GetPosts(PostQueryDto postDto);
        Task<ApiResponse<Post>> InsertPost(PostCreateDto postDto);
        Task<ApiResponse> DeletePost(PostDeleteDto postDto);
    }

    public class PostService : BaseService, IPostService
    {
        public PostService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor) : base(mainUnitOfWork, httpContextAccessor)
        {
        }

        public async Task<ApiResponse> DeletePost(PostDeleteDto postDto)
        {
            var existingPost = await MainUnitOfWork.PostRepository.FindOneAsync(postDto.Id);
            if (existingPost != null)
            {
                return ApiResponse.Failed();
            }

            bool isDeleted = await MainUnitOfWork.PostRepository.DeleteAsync(existingPost, AccountId);

            if (isDeleted)
            {
                return ApiResponse.Success("Successfully");
            }
            else
            {
                return ApiResponse.Failed();
            }
        }

        public async Task<ApiResponses<PostDto>> GetPosts(PostQueryDto postDto)
        {
            var response = await MainUnitOfWork.PostRepository.FindResultAsync<PostDto>(new Expression<Func<Post, bool>>[]
            {
                x => !x.DeletedAt.HasValue
            }, postDto.OrderBy, postDto.Skip(), postDto.PageSize);

            return ApiResponses<PostDto>.Success(response.Items,
                response.TotalCount,
                postDto.PageSize,
                postDto.Skip(),
                (int)Math.Ceiling(response.TotalCount / (double)postDto.PageSize));
        }

        public async Task<ApiResponse<Post>> InsertPost(PostCreateDto postDto)
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
                return ApiResponse<Post>.Success(post);
            }
            else
            {
                return (ApiResponse<Post>)ApiResponse.Failed();
            }
        }
    }
}
