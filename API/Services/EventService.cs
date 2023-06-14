using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API.Services;
public interface IEventService : IBaseService
{
    Task<ApiResponses<EventDto>> GetListEvents(EventQueryDto queryDto);
    Task<ApiResponse<EventDetailDto>> GetEvent(Guid id);
    Task<ApiResponse<EventDetailDto>> Create(EventCreateDto eventsDto);
    Task<ApiResponse<EventDetailDto>> Update(Guid id, EventUpdateDto eventsUpdateDto);
    Task<ApiResponse> Delete(Guid id);
}
public class EventService : BaseService, IEventService
{
    public EventService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse<EventDetailDto>> Create(EventCreateDto eventsDto)
    {
        if (eventsDto.MaxParticipants < eventsDto.MinParticipants)
        {
            throw new ApiException("MaxParticipants must be greater than MinParticipants", StatusCode.BAD_REQUEST);
        }
        if (eventsDto.StartDate >= eventsDto.EndDate)
        {
            throw new ApiException("StartDate must be less than EndDate", StatusCode.BAD_REQUEST);
        }
        var events = eventsDto.ProjectTo<EventCreateDto, Event>();

        events.Id = Guid.Empty;

        if (!await MainUnitOfWork.EventRepository.InsertAsync(events, AccountId, CurrentDate))
            throw new ApiException("Can't create", StatusCode.SERVER_ERROR);

        return await GetEvent(events.Id);
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingEvent = await MainUnitOfWork.EventRepository.FindOneAsync(id);
        if (existingEvent == null)
            throw new ApiException("Not found this events", StatusCode.NOT_FOUND);

        if (existingEvent.CreatorId != AccountId)
            throw new ApiException("Can't not delete other's events", StatusCode.BAD_REQUEST);

        if (await MainUnitOfWork.EventRepository.DeleteAsync(existingEvent, AccountId, CurrentDate))
            throw new ApiException("Delete SucessFull", StatusCode.SUCCESS);

        return ApiResponse.Success();
    }

    public async Task<ApiResponse<EventDetailDto>> GetEvent(Guid id)
    {
        var existingEvent = await MainUnitOfWork.EventRepository.FindOneAsync<EventDetailDto>(
                new Expression<Func<Event, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });


        if (existingEvent == null)
            throw new ApiException("Not found this events", StatusCode.NOT_FOUND);

        return ApiResponse<EventDetailDto>.Success(existingEvent);
    }

    public async Task<ApiResponses<EventDto>> GetListEvents(EventQueryDto queryDto)
    {
        // Get list
        var eventss = await MainUnitOfWork.EventRepository.FindResultAsync<EventDto>(new Expression<Func<Event, bool>>[]
        {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.EventName) || x.EventName.Trim().ToLower().Contains(queryDto.EventName.Trim().ToLower())
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        
        // Map to get CDC
        eventss.Items = await _mapperRepository.MapCreator(eventss.Items.ToList());

        // Map total participant to each event
        var participant = MainUnitOfWork.ParticipantRepository.GetQuery();
        

        // Map feedbacks to each post
        var feedbacks = MainUnitOfWork.FeedbackRepository.GetQuery();
        foreach (var events in eventss.Items)
        {
            events.TotalParticipant = participant.Count(x => x!.EventId == events.Id);
            events.TotalFeedback = feedbacks.Count(x => x!.EventId == events.Id);
        }

        return ApiResponses<EventDto>.Success(
            eventss.Items,
            eventss.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(eventss.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<EventDetailDto>> Update(Guid id, EventUpdateDto eventsUpdateDto)
    {
        var events = await MainUnitOfWork.EventRepository.FindOneAsync(id);
        if (events == null)
            throw new ApiException("Not found this events", StatusCode.NOT_FOUND);

        if (events.CreatorId != AccountId)
            throw new ApiException("Can't not update other's events", StatusCode.BAD_REQUEST);

        events.EventName = eventsUpdateDto.EventName ?? events.EventName;
        events.Status = eventsUpdateDto.Status != null ? eventsUpdateDto.Status : events.Status;
        events.Type = eventsUpdateDto.Type != null ? eventsUpdateDto.Type : events.Type;
        events.CoverImage = eventsUpdateDto.CoverImage ?? events.CoverImage;
        events.Description = eventsUpdateDto.Description ?? events.Description;
        events.MaxParticipants = eventsUpdateDto.MaxParticipants != 0 ? eventsUpdateDto.MaxParticipants : events.MaxParticipants;
        events.MinParticipants = eventsUpdateDto.MinParticipants != 0 ? eventsUpdateDto.MinParticipants : events.MinParticipants;
        events.StartDate = eventsUpdateDto.StartDate != DateTime.MinValue ? eventsUpdateDto.StartDate : events.StartDate;
        events.EndDate = eventsUpdateDto.EndDate != DateTime.MinValue ? eventsUpdateDto.EndDate : events.EndDate;
        events.Location = eventsUpdateDto.Location ?? events.Location;
        events.Prerequisite = eventsUpdateDto.Prerequisite ?? events.Prerequisite;
        events.EvaluationStrategy = eventsUpdateDto.EvaluationStrategy ?? events.EvaluationStrategy;

        if (eventsUpdateDto.MaxParticipants < eventsUpdateDto.MinParticipants)
        {
            throw new ApiException("MaxParticipants must be greater than MinParticipants", StatusCode.BAD_REQUEST);
        }
        if (eventsUpdateDto.StartDate >= eventsUpdateDto.EndDate)
        {
            throw new ApiException("StartDate must be less than EndDate", StatusCode.BAD_REQUEST);
        }
        if (!await MainUnitOfWork.EventRepository.UpdateAsync(events, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return await GetEvent(id);
    }
}