using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace API.Services
{
    public interface ICommentService : IBaseService
    {
        Task<ApiResponse<TotalCommentDto>> AddComment(CommentCreateDto commentDto);
        Task<ApiResponse<CommentCreateDto>> UpdateComment(Guid id, CommentCreateDto commentDto);
        Task<ApiResponse<CommentDeleteDto>> DeleteComment(Guid id, CommentDeleteDto commentDto);
    }

    public class CommentService : BaseService, ICommentService
    {
        public CommentService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse<TotalCommentDto>> AddComment(CommentCreateDto commentDto)
        {
            var post = await MainUnitOfWork.PostRepository.FindOneAsync(commentDto.PostId);
            var replyCmt = await MainUnitOfWork.CommentRepository.FindOneAsync(commentDto.ReplyTo);

            if (post == null)
                throw new ApiException("Post not found", StatusCode.NOT_FOUND);

            if (replyCmt != null && replyCmt.PostId != post.Id)
                throw new ApiException("Parent comment not found", StatusCode.NOT_FOUND);

            var comment = commentDto.ProjectTo<CommentCreateDto, Comment>();

            if(replyCmt != null && post != null && replyCmt.PostId == post.Id)
            {
                comment.ReplyTo = commentDto.ReplyTo;
            }  

            var success = await MainUnitOfWork.CommentRepository.InsertAsync(comment, AccountId, CurrentDate);

            if (!success)
                throw new ApiException("Failed to add comment", StatusCode.SERVER_ERROR);

            var totalComment = MainUnitOfWork.CommentRepository.GetQuery().Where(X =>
                !X!.DeletedAt.HasValue);

            var total = new TotalCommentDto { TotalComemnts = totalComment.Count(x => x.PostId == post.Id) };


            return ApiResponse<TotalCommentDto>.Success(total);
        }

        public async Task<ApiResponse<CommentDeleteDto>> DeleteComment(Guid id, CommentDeleteDto commentDto)
        {
            var existingPost = await MainUnitOfWork.PostRepository.FindOneAsync(commentDto.PostId);
            var existingComment = await MainUnitOfWork.CommentRepository.FindOneAsync(id);

            if (existingComment == null)
                throw new ApiException("Not found this comment", StatusCode.NOT_FOUND);

            if (existingComment.CreatorId != AccountId || existingPost.CreatorId != AccountId)
                throw new ApiException("Can't not delete other's commemnt", StatusCode.BAD_REQUEST);

            var result = await MainUnitOfWork.CommentRepository.DeleteAsync(existingComment, AccountId, CurrentDate);
            if (!result)
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

            return (ApiResponse<CommentDeleteDto>)ApiResponse.Success();
        }

        public async Task<ApiResponse<CommentCreateDto>> UpdateComment(Guid id, CommentCreateDto commentDto)
        {
            var existingComment = await MainUnitOfWork.CommentRepository.FindOneAsync(id);

            if (existingComment == null)
                throw new ApiException("Comment not found", StatusCode.NOT_FOUND);

            if (existingComment.CreatorId != AccountId)
                throw new ApiException("Can't not update other's comment", StatusCode.BAD_REQUEST);

            existingComment.Content = commentDto.Content ?? existingComment.Content;

            var result = await MainUnitOfWork.CommentRepository.UpdateAsync(existingComment, AccountId, CurrentDate);

            if (!result)
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

            return (ApiResponse<CommentCreateDto>)ApiResponse.Success();
        }
    }
}
