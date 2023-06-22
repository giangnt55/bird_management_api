using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API.Services
{
    public interface ILikeService : IBaseService
    {
        Task<ApiResponse<LikeResultDto>> LikePost(LikePostDto likeDto);
        Task<ApiResponse> LikeComment(LikeCommentDto likeDto);
    }

    public class LikeService : BaseService, ILikeService
    {
        public LikeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> LikeComment(LikeCommentDto likeDto)
        {
            //var post = await MainUnitOfWork.PostRepository.FindOneAsync(likeDto.PostId);
            var comment = await MainUnitOfWork.CommentRepository.FindOneAsync(likeDto.CommentId);
            //if (post == null)
            //    throw new ApiException("Post not found", StatusCode.NOT_FOUND);

            if (comment == null)
                throw new ApiException("Comment not found", StatusCode.NOT_FOUND);

            var existingLike = await MainUnitOfWork.LikeRepository.FindOneAsync(new Expression<Func<Like, bool>>[]
                {
                    x => x.CommentId == comment.Id && x.CreatorId == AccountId
                });

            if (existingLike != null)
            {
                if (!await MainUnitOfWork.LikeRepository.DeleteAsync(existingLike, AccountId, CurrentDate))
                    throw new ApiException("Failed to remove the like", StatusCode.SERVER_ERROR);
            }
            else
            {
                var like = likeDto.ProjectTo<LikeCommentDto, Like>();

                if (!await MainUnitOfWork.LikeRepository.InsertAsync(like, AccountId, CurrentDate))
                    throw new ApiException("Failed to like the post", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success(); //CHUA BIET TRA VE GI

        }

        public async Task<ApiResponse<LikeResultDto>> LikePost(LikePostDto likeDto)
        {
            var post = await MainUnitOfWork.PostRepository.FindOneAsync(likeDto.PostId);
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
                var like = likeDto.ProjectTo<LikePostDto, Like>();

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
