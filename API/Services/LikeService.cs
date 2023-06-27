using System.Linq.Expressions;
using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API.Services;

public interface ILikeService : IBaseService
{
  public Task<ApiResponse> CreateLike(CreateLikeDto createLikeDto);

  public Task<ApiResponse> Unlike(CreateLikeDto createLikeDto);

  public Task<ApiResponses<LikeDto>> GetLikeOfPost(Guid postId);
}

public class LikeService : BaseService, ILikeService
{
  public LikeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
  {
  }

  public async Task<ApiResponse> CreateLike(CreateLikeDto createLikeDto)
  {

    var post = await MainUnitOfWork.PostRepository.FindOneAsync(new Expression<Func<Post, bool>>[]
    {
      x => x.Id == createLikeDto.PostId,
      x => !x.DeletedAt.HasValue
    });

    var comment = await MainUnitOfWork.CommentRepository.FindOneAsync(new Expression<Func<Comment, bool>>[]
    {
      x => x.Id == createLikeDto.CommentId,
      x => !x.DeletedAt.HasValue
    });

    if (post == null && comment == null)
      throw new ApiException("Can't not like this content", StatusCode.BAD_REQUEST);

    // Check if the user has already liked the post or the comment
    var existingLike = await MainUnitOfWork.LikeRepository.FindOneAsync(new Expression<Func<Like, bool>>[]
    {
      x => x.CreatorId == AccountId &&
           (
             (post != null && x.PostId == post.Id) ||
             (comment != null && x.CommentId == comment.Id)
           )
    });

    if (existingLike != null)
    {
      existingLike.DeletedAt = null;
      existingLike.UpdatedAt = CurrentDate;

      if(!await MainUnitOfWork.LikeRepository.UpdateAsync(existingLike, AccountId, CurrentDate))
        throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
    }
    else
    {
      // Create a new Like entity
      var like = new Like();
      if (post != null)
        like.PostId = post.Id;

      if (comment != null)
        like.CommentId = comment.Id;

      if (!await MainUnitOfWork.LikeRepository.InsertAsync(like, AccountId, CurrentDate))
        throw new ApiException("Fail to like this post", StatusCode.SERVER_ERROR);
    }

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> Unlike(CreateLikeDto createLikeDto)
  {
    var like = await MainUnitOfWork.LikeRepository.FindOneAsync(new Expression<Func<Like, bool>>[]
    {
      x => !x.DeletedAt.HasValue,
      x => x.CreatorId == AccountId &&
           (
             (createLikeDto.PostId != null && x.PostId == createLikeDto.PostId) ||
             (createLikeDto.CommentId != null && x.CommentId == createLikeDto.CommentId)
           )
    });

    if (like == null)
      throw new ApiException("Not found this content", StatusCode.NOT_FOUND);

    if(!await MainUnitOfWork.LikeRepository.DeleteAsync(like, AccountId, CurrentDate))
      throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);

    return ApiResponse.Success();
  }

  public async Task<ApiResponses<LikeDto>> GetLikeOfPost(Guid postId)
  {
    var likes = await MainUnitOfWork.LikeRepository.FindAsync<LikeDto>(new Expression<Func<Like, bool>>[]
    {
        x => !x.DeletedAt.HasValue,
        x => x.PostId == postId
    }, null);

    likes = await _mapperRepository.MapCreator(likes);

    return ApiResponses<LikeDto>.Success(likes);
  }
}
