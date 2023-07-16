using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API.Services
{
    public interface IFeedbackService : IBaseService
    {
        Task<ApiResponses<FeedbackDto>> GetListFeedbacks(FeedbackQueryDto queryDto);
        Task<ApiResponse<FeedbackDetailDto>> GetFeedback(Guid id);
        Task<ApiResponse<FeedbackDetailDto>> Create(FeedbackCreateDto feedbackDto);
        Task<ApiResponse<FeedbackDetailDto>> Update(Guid id, FeedbackUpdateDto feedbackUpdateDto);
        Task<ApiResponse> Delete(Guid id);
    }
    public class FeedbackService : BaseService, IFeedbackService
    {

        public FeedbackService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse<FeedbackDetailDto>> Update(Guid id, FeedbackUpdateDto feedbackUpdateDto)
        {
            var feedback = await MainUnitOfWork.FeedbackRepository.FindOneAsync(id);
            if (feedback == null)
                throw new ApiException("Not found this feedback", StatusCode.NOT_FOUND);

            if (feedback.CreatorId != AccountId)
                throw new ApiException("Can't not update other's feedback", StatusCode.BAD_REQUEST);

            var checkParticipant = MainUnitOfWork.ParticipantRepository.GetQuery().Where(x => x.Id == feedback.ParticipantId).FirstOrDefault();
            if (checkParticipant == null)
            {
                throw new ApiException("Participant not Found, please check again", StatusCode.NOT_FOUND);
            }
            else
            {
                feedback.ParticipantId = feedbackUpdateDto.ParticipantId;
            }
            var checkEvent = MainUnitOfWork.EventRepository.GetQuery().Where(x => x.Id == feedback.EventId).FirstOrDefault();
            if (checkEvent == null)
            {
                throw new ApiException("Event not Found, please check again", StatusCode.NOT_FOUND);
            }
            else
            {
                feedback.EventId = feedbackUpdateDto.EventId;
            }
            feedback.Content = feedbackUpdateDto.Content ?? feedback.Content;
            if (feedbackUpdateDto.Rating != null)
            {
                feedback.Rating = (RateStar?)feedbackUpdateDto.Rating;
            }
            if (!await MainUnitOfWork.FeedbackRepository.UpdateAsync(feedback, AccountId, CurrentDate))
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

            return await GetFeedback(id);
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingFeedback = await MainUnitOfWork.FeedbackRepository.FindOneAsync(id);
            if (existingFeedback == null)
                throw new ApiException("Not found this feedback", StatusCode.NOT_FOUND);

            if (existingFeedback.CreatorId != AccountId)
                throw new ApiException("Can't not delete other's feedback", StatusCode.BAD_REQUEST);

            if (await MainUnitOfWork.FeedbackRepository.DeleteAsync(existingFeedback, AccountId, CurrentDate))
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

            return ApiResponse.Success();
        }


        public async Task<ApiResponses<FeedbackDto>> GetListFeedbacks(FeedbackQueryDto queryDto)
        {
            // Get list
            var feedbacks = await MainUnitOfWork.FeedbackRepository.FindResultAsync<FeedbackDto>(new Expression<Func<FeedBack, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.Content) || x.Content.Trim().ToLower().Contains(queryDto.Content.Trim().ToLower()),
                x => queryDto.EventId == Guid.Empty || x.EventId == queryDto.EventId
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);


            feedbacks.Items = await _mapperRepository.MapCreator(feedbacks.Items.ToList());

            return ApiResponses<FeedbackDto>.Success(
                feedbacks.Items,
                feedbacks.TotalCount, 
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(feedbacks.TotalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<FeedbackDetailDto>> GetFeedback(Guid id)
        {
            var feedback = await MainUnitOfWork.FeedbackRepository.FindOneAsync<FeedbackDetailDto>(
                new Expression<Func<FeedBack, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (feedback == null)
                throw new ApiException("Not found this feedback", StatusCode.NOT_FOUND);
            return ApiResponse<FeedbackDetailDto>.Success(feedback);
        }

        public async Task<ApiResponse<FeedbackDetailDto>> Create(FeedbackCreateDto feedbackDto)
        {
            var feedback = feedbackDto.ProjectTo<FeedbackCreateDto, FeedBack>();

            var checkParticipant = MainUnitOfWork.ParticipantRepository.GetQuery()
                .Where(x => x!.EventId == feedback.EventId && x.CreatorId == AccountId && !x.DeletedAt.HasValue).FirstOrDefault();
            if (checkParticipant == null)
            {
                throw new ApiException("Not have permit to feedback this event", StatusCode.BAD_REQUEST);
            }

            feedback.ParticipantId = checkParticipant.Id;
            
            if (!await MainUnitOfWork.FeedbackRepository.InsertAsync(feedback, AccountId, CurrentDate))
                throw new ApiException("Can't create", StatusCode.SERVER_ERROR);

            return await GetFeedback(feedback.Id);
        }
    }
}
