using API.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using System.Linq.Expressions;
using AppCore.Extensions;
using MainData.Repositories;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Services
{
    public interface IPostService : IBaseService 
    {
        Task<ApiResponses<PostDto>> GetPosts(PostQueryDto queryDto);
        Task<ApiResponse<DetailPostDto>> GetPost(Guid id);
        Task<ApiResponse<DetailPostDto>> Create(PostCreateDto postDto);
        Task<ApiResponse<DetailPostDto>> Update(Guid id, PostUpdateDto postUpdateDto);
        Task<ApiResponse> Delete(Guid id);
    }

    public class PostService : BaseService, IPostService
    {
        public PostService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse<DetailPostDto>> Update(Guid id, PostUpdateDto postUpdateDto)
        {
            var post = await MainUnitOfWork.PostRepository.FindOneAsync(id);
            if (post == null)
                throw new ApiException("Not found this post", StatusCode.NOT_FOUND);

            if (post.CreatorId != AccountId)
                throw new ApiException("Can't not update other's post", StatusCode.BAD_REQUEST);

            post.Content = postUpdateDto.Content ?? post.Content;
            post.Tittle = postUpdateDto.Tittle ?? post.Tittle;
            post.Image = postUpdateDto.Image ?? post.Image;

            if (!await MainUnitOfWork.PostRepository.InsertAsync(post, AccountId, CurrentDate))
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

            return await GetPost(id);
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingPost = await MainUnitOfWork.PostRepository.FindOneAsync(id);
            if (existingPost == null)
                throw new ApiException("Not found this post", StatusCode.NOT_FOUND);

            if (existingPost.CreatorId != AccountId)
                throw new ApiException("Can't not delete other's post", StatusCode.BAD_REQUEST);
            
            if (await MainUnitOfWork.PostRepository.DeleteAsync(existingPost, AccountId, CurrentDate))
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Success();
        }


        public async Task<ApiResponses<PostDto>> GetPosts(PostQueryDto queryDto)
        {
            // Get list
            var posts = await MainUnitOfWork.PostRepository.FindResultAsync<PostDto>(new Expression<Func<Post, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            // Map to get CDC
            posts.Items = await _mapperRepository.MapCreator(posts.Items.ToList());

            // Map total like to each post
            var likes = MainUnitOfWork.LikeRepository.GetQuery();

            foreach (var post in posts.Items)
            {
                post.TotalLike = likes.Count(x => x!.TargetId == post.Id);
            }
            
            // Map comments to each post
            var comments = MainUnitOfWork.CommentRepository.GetQuery();
            
            foreach (var post in posts.Items)
            {
                post.TotalComment = likes.Count(x => x!.TargetId == post.Id);
            }

            return ApiResponses<PostDto>.Success(
                posts.Items,
                posts.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(posts.TotalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<DetailPostDto>> GetPost(Guid id)
        {
            var post = await MainUnitOfWork.PostRepository.FindOneAsync<DetailPostDto>(
                new Expression<Func<Post, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (post == null)
                throw new ApiException("Not found this post", StatusCode.NOT_FOUND);

            post.TotalLike = MainUnitOfWork.LikeRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
                && x.TargetId == post.Id);
            
            // Map CDC for the post
            post = await _mapperRepository.MapCreator(post);

            // Map comments of the post
            var comments = await MainUnitOfWork.CommentRepository.FindAsync<DetailCommentDto>(
                new Expression<Func<Comment, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.PostId == post.Id,
                    x => x.ReplyTo == null || x.ReplyTo == Guid.Empty
                }, null);
            
            // Map CDC for comment
            comments = await _mapperRepository.MapCreator(comments);

            post.TotalComment = comments.Count();

            // Map replies for each comment
            var commentDataset = MainUnitOfWork.CommentRepository.GetQuery().Where(x=> 
                !x!.DeletedAt.HasValue);
            var likeDataSet = MainUnitOfWork.LikeRepository.GetQuery().Where(x=> 
                !x!.DeletedAt.HasValue);
            
            foreach (var comment in comments)
            {
                var repliesOfComments = commentDataset.Where(x => x!.ReplyTo == comment.Id)
                    .ToList()!.ProjectTo<Comment, CommentDto>();

                foreach (var reply in repliesOfComments)
                {
                    reply.TotalLike = likeDataSet.Count(x => x!.TargetId == reply.Id);
                }

                // Map CDC for replies
                repliesOfComments = await _mapperRepository.MapCreator(repliesOfComments);
                comment.Replies = repliesOfComments;
                
                // Map total like for comment
                comment.TotalLike = likeDataSet.Count(x => x!.TargetId == comment.Id);
            }

            return ApiResponse<DetailPostDto>.Success(post);
        }

        public async Task<ApiResponse<DetailPostDto>> Create(PostCreateDto postDto)
        {
            var post = postDto.ProjectTo<PostCreateDto, Post>();
            
            post.Id = Guid.Empty;

            if (!await MainUnitOfWork.PostRepository.InsertAsync(post, AccountId, CurrentDate))
                throw new ApiException("Can't create", StatusCode.SERVER_ERROR);

            return await GetPost(post.Id);
        }
        
    }
}
