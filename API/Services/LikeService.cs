using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API.Services
{
    public interface ILikeService : IBaseService
    {
        Task<ApiResponse<LikeResultDto>> LikePost(Guid id);
    }

    public class LikeService : BaseService, ILikeService
    {
        public LikeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse<LikeResultDto>> LikePost(Guid id)
        {
            var post = await MainUnitOfWork.PostRepository.FindOneAsync(id);
            if (post == null)
                throw new ApiException("Post not found", StatusCode.NOT_FOUND);

            // Check if the user has already liked the post
            var existingLike = await MainUnitOfWork.LikeRepository.FindOneAsync(new Expression<Func<Like, bool>>[]
                {
                    x => x.PostId == post.Id && x.CreatorId == AccountId
                });

            if (existingLike != null)
            {
                // User has already liked the post, so remove the like
                if (!await MainUnitOfWork.LikeRepository.DeleteAsync(existingLike, AccountId, CurrentDate))
                    throw new ApiException("Failed to remove the like", StatusCode.SERVER_ERROR);
            }
            else
            {
                // Create a new Like entity
                var like = new Like
                {
                    PostId = post.Id,
                    CreatorId = AccountId,
                    CreatedAt = CurrentDate
                };

                if (!await MainUnitOfWork.LikeRepository.InsertAsync(like, AccountId, CurrentDate))
                    throw new ApiException("Failed to like the post", StatusCode.SERVER_ERROR);
            }

            // Retrieve the updated total number of likes for the post
            var totalLikes = MainUnitOfWork.LikeRepository.GetQuery().Where(x =>
                !x!.DeletedAt.HasValue);

            var resultDto = new LikeResultDto
            {
                TotalLikes = totalLikes.Count(x => x.PostId == post.Id)
            };

            return ApiResponse<LikeResultDto>.Success(resultDto);
        }
    }
}
