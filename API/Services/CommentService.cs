using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using AppCore.Extensions;

namespace API.Services
{
    public interface ICommentService : IBaseService
    {
        Task<ApiResponse> AddComment(CommentCreateDto commentDto);
       // Task<ApiResponse<CommentCreateDto>> UpdateComment(Guid id, CommentCreateDto commentDto);
        Task<ApiResponse> DeleteComment(Guid id);
        Task<ApiResponses<CommentDto>> GetCommentByPost(Guid postId);
    }

    public class CommentService : BaseService, ICommentService
    {
        public CommentService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> AddComment(CommentCreateDto commentDto)
        {
          var post = await MainUnitOfWork.PostRepository.FindOneAsync(new Expression<Func<Post, bool>>[]
          {
            x => x.Id == commentDto.PostId,
            x => !x.DeletedAt.HasValue
          });

          var targetComment = await MainUnitOfWork.CommentRepository.FindOneAsync(new Expression<Func<Comment, bool>>[]
          {
            x => x.Id == commentDto.ReplyTo,
            x => !x.DeletedAt.HasValue
          });

          if (post == null && targetComment == null)
            throw new ApiException("Not found this content", StatusCode.NOT_FOUND);

          var comment = new Comment
          {
            PostId = commentDto.PostId,
            ReplyTo = commentDto.ReplyTo,
            Content = commentDto.Content
          };

          if (!await MainUnitOfWork.CommentRepository.InsertAsync(comment, AccountId, CurrentDate))
              throw new ApiException("Failed to add comment", StatusCode.SERVER_ERROR);

          return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteComment(Guid id)
        {
            var existingComment = await MainUnitOfWork.CommentRepository.FindOneAsync(id);
            if (existingComment == null)
                throw new ApiException("Not found this comment", StatusCode.NOT_FOUND);

            if (existingComment.CreatorId != AccountId)
                throw new ApiException("Can't not delete other's commemnt", StatusCode.BAD_REQUEST);

            var result = await MainUnitOfWork.CommentRepository.DeleteAsync(existingComment, AccountId, CurrentDate);
            if (!result)
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

            return ApiResponse.Success();
        }

        public async Task<ApiResponses<CommentDto>> GetCommentByPost(Guid postId)
        {
          var comments = await MainUnitOfWork.CommentRepository.FindAsync<CommentDto>(
            new Expression<Func<Comment, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.PostId == postId,
            }, null);


          // Map replies for each comment
          var commentDataset = MainUnitOfWork.CommentRepository.GetQuery().Where(x =>
            !x!.DeletedAt.HasValue);
          var likeDataSet = MainUnitOfWork.LikeRepository.GetQuery().Where(x =>
            !x!.DeletedAt.HasValue);

          foreach (var comment in comments)
          {
            var repliesOfComments = commentDataset.Where(x => x!.ReplyTo == comment.Id)
              .ToList()!.ProjectTo<Comment, CommentDto>();

            foreach (var reply in repliesOfComments)
            {
              reply.TotalLike = likeDataSet.Count(x => x!.PostId == reply.Id);
            }

            // Map CDC for replies
            repliesOfComments = await _mapperRepository.MapCreator(repliesOfComments);
            comment.Replies = repliesOfComments;

            // Map total like for comment
            comment.TotalLike = likeDataSet.Count(x => x!.PostId == comment.Id);
          }

          return ApiResponses<CommentDto>.Success(comments);
        }

        // public async Task<ApiResponse<CommentCreateDto>> UpdateComment(Guid id, CommentCreateDto commentDto)
        // {
        //     var existingComment = await MainUnitOfWork.CommentRepository.FindOneAsync(id);
        //
        //     if (existingComment == null)
        //         throw new ApiException("Comment not found", StatusCode.NOT_FOUND);
        //
        //     if (existingComment.CreatorId != AccountId)
        //         throw new ApiException("Can't not update other's comment", StatusCode.BAD_REQUEST);
        //
        //     existingComment.Content = commentDto.Content ?? existingComment.Content;
        //
        //     var result = await MainUnitOfWork.CommentRepository.UpdateAsync(existingComment, AccountId, CurrentDate);
        //
        //     if (!result)
        //         throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
        //
        //     return (ApiResponse<CommentCreateDto>)ApiResponse.Success();
        // }
    }
}
