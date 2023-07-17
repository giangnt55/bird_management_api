using API.Dtos;
using AppCore.Models;
using MainData.Repositories;
using MainData;
using AppCore.Extensions;
using MainData.Entities;
using System.Linq.Expressions;

namespace API.Services;
public interface IParticipantService : IBaseService
{
    Task<ApiResponses<ParticipantDto>> GetParticipants(ParticipantQueryDto queryDto);
    Task<ApiResponse<DetailParticipantDto>> Create(ParticipantCreateDto participantDto);
    Task<ApiResponse<Participant>> UpdateParticipant(Guid Id, ParticipantUpdateDto participantDto);
    Task<ApiResponse> Delete(Guid eventId);
}
public class ParticipantService : BaseService, IParticipantService
{
    public ParticipantService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse<Participant>> UpdateParticipant(Guid Id, ParticipantUpdateDto participantDto)
    {
        var existingParticipant = await MainUnitOfWork.ParticipantRepository.FindOneAsync(Id);

        if (existingParticipant == null)
        {
            return (ApiResponse<Participant>)ApiResponse.Failed();
        }

        var participant = existingParticipant;

        existingParticipant.Role = participantDto.Role;

        bool isUpdated = await MainUnitOfWork.ParticipantRepository.UpdateAsync(participant, AccountId, CurrentDate);

        if (isUpdated)
        {
            return ApiResponse<Participant>.Success(participant);
        }
        else
        {
            return (ApiResponse<Participant>)ApiResponse.Failed();
        }
    }
    public async Task<ApiResponse> Delete(Guid eventId)
    {
        var existingParticipant = await MainUnitOfWork.ParticipantRepository.FindOneAsync(new Expression<Func<Participant, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.EventId == eventId,
            x => x.CreatorId == AccountId
        });
        if (existingParticipant == null)
            throw new ApiException("Not found this participant", StatusCode.NOT_FOUND);

        if (!await MainUnitOfWork.ParticipantRepository.DeleteAsync(existingParticipant, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }


    public async Task<ApiResponses<ParticipantDto>> GetParticipants(ParticipantQueryDto queryDto)
    {
        // Get list
        var participants = await MainUnitOfWork.ParticipantRepository.FindResultAsync<ParticipantDto>(new Expression<Func<Participant, bool>>[]
        {
                x => !x.DeletedAt.HasValue,
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        // Map to get CDC
        participants.Items = await _mapperRepository.MapCreator(participants.Items.ToList());

        return ApiResponses<ParticipantDto>.Success(
            participants.Items,
            participants.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(participants.TotalCount / (double)queryDto.PageSize)
        );
    }


    public async Task<ApiResponse<DetailParticipantDto>> GetParticipant(Guid id)
    {
        var participant = await MainUnitOfWork.ParticipantRepository.FindOneAsync<DetailParticipantDto>(
            new Expression<Func<Participant, bool>>[]
            {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
            });

        if (participant == null)
            throw new ApiException("Not found this participant", StatusCode.NOT_FOUND);
        return ApiResponse<DetailParticipantDto>.Success(participant);
    }
    public async Task<ApiResponse<DetailParticipantDto>> Create(ParticipantCreateDto participantDto)
    {

        // check account
        var checkCreator = MainUnitOfWork.ParticipantRepository.GetQuery().Where(x => x.CreatorId == AccountId && participantDto.EventId == x.EventId);
        if (checkCreator.Count() != 0)
        {
            throw new ApiException("Participant already has an account", StatusCode.BAD_REQUEST);
        }

        var participant = participantDto.ProjectTo<ParticipantCreateDto, Participant>();
        // var existingParticipant = await GetParticipant(participant.Id);
        // if (existingParticipant == null)
        //     throw new ApiException("Can't create", StatusCode.SERVER_ERROR);

        // count participant to check max join and role
        var countParticipant = MainUnitOfWork.ParticipantRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue && x.EventId == participantDto.EventId && x.Role == ParticipantRole.Participant);

        // get maxparti in event
        var existingEvent = await MainUnitOfWork.EventRepository.FindOneAsync<EventDetailDto>(
               new Expression<Func<Event, bool>>[]
               {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == participantDto.EventId
               });

        // check count and max
        if (countParticipant > existingEvent.MaxParticipants)
        {
            throw new ApiException("Participant was fully", StatusCode.BAD_REQUEST);
        }
        
        if (existingEvent.Status != EventStatus.UpComing) 
        {
            throw new ApiException("Time to join this event has expired", StatusCode.BAD_REQUEST);
        }
        
        participant.Role = ParticipantRole.Participant;
        //insert
        if (!await MainUnitOfWork.ParticipantRepository.InsertAsync(participant, AccountId))
            throw new ApiException("Can't create", StatusCode.SERVER_ERROR);

        return await GetParticipant(participant.Id);
    }
}
