using API.Dtos;
using AppCore.Models;
using MainData.Entities;
using MainData.Repositories;
using MainData;
using System;
using AppCore.Extensions;
using System.Linq.Expressions;
using AutoMapper;
namespace API.Services
{
    public interface INotificationService : IBaseService
    {
        Task<ApiResponse<DetailNotificationDto>> CreateNotification(CreateNotification notificationDto);
        Task<ApiResponse<DetailNotificationDto>> GetNotification(Guid id);
        Task<ApiResponse<DetailNotificationDto>> GetNotificationId(Guid id);
        Task<ApiResponses <NotificationDto>> GetNotifications(NotificationQueryDto queryDto);

    }
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }
        public async Task<ApiResponse<DetailNotificationDto>> GetNotification(Guid id)
        {
            var notification = await MainUnitOfWork.NotificationRepository.FindOneAsync<DetailNotificationDto>(
                new Expression<Func<Notification, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (notification == null)
                throw new ApiException("Not found this notification", StatusCode.NOT_FOUND);


            // Map CDC for the Notification
            notification = await _mapperRepository.MapCreator(notification);

            return ApiResponse<DetailNotificationDto>.Success(notification);
        }

        public async Task<ApiResponse<DetailNotificationDto>> GetNotificationId(Guid id)
        {
            var notificationExisting = await MainUnitOfWork.NotificationRepository.FindOneAsync<UpdateNotification>(
                new Expression<Func<Notification, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            await UpdateIsReadNotification(id, notificationExisting);
            if (notificationExisting == null)
                throw new ApiException("Not found this notification", StatusCode.NOT_FOUND);

            var notification = await MainUnitOfWork.NotificationRepository.FindOneAsync<DetailNotificationDto>(
                new Expression<Func<Notification, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            // Map CDC for the Notification
            notification = await _mapperRepository.MapCreator(notification);

            return ApiResponse<DetailNotificationDto>.Success(notification);
        }

        public async Task<ApiResponse<Notification>> UpdateIsReadNotification(Guid Id, UpdateNotification notificationDto)
        {
            var existingNotification = await MainUnitOfWork.NotificationRepository.FindOneAsync(Id);

            if (existingNotification == null)
            {
                return (ApiResponse<Notification>)ApiResponse.Failed();
            }
            var notification = existingNotification;
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            bool isUpdated = await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, AccountId);

            if (isUpdated)
            {
                return ApiResponse<Notification>.Success(notification);
            }
            else
            {
                return (ApiResponse<Notification>)ApiResponse.Failed();
            }
        }

        public async Task<ApiResponse<DetailNotificationDto>> CreateNotification(CreateNotification notificationDto)
        {
            var notification = notificationDto.ProjectTo<CreateNotification, Notification>();

            notification.Id = Guid.Empty;
            notification.IsRead = false;
            if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification,AccountId))
                throw new ApiException("Can't create", StatusCode.SERVER_ERROR);
            // Create a response DTO to send back to the client
            var responseDto = new ApiResponse<DetailNotificationDto>();

            // Set the response status and message
            responseDto.Message = "Notification created successfully";

            // Map the created notification to the response DTO using AutoMapper
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Notification, DetailNotificationDto>());
            var mapper = mapperConfig.CreateMapper();
            responseDto.Data = mapper.Map<DetailNotificationDto>(notification);

            // Send the response back to the client
            return responseDto;
        }
        public async Task<ApiResponses <NotificationDto>> GetNotifications(NotificationQueryDto queryDto)
        {
            // Get list
            var notifications = await MainUnitOfWork.NotificationRepository.FindResultAsync<NotificationDto>(new Expression<Func<Notification, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.Content) || x.Content.Trim().ToLower().Contains(queryDto.Content.Trim().ToLower()),
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            // Map to get CDC
            notifications.Items = await _mapperRepository.MapCreator(notifications.Items.ToList());

           

            return ApiResponses<NotificationDto>.Success(
                notifications.Items,
                notifications.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(notifications.TotalCount / (double)queryDto.PageSize)
            );
        }
        public async Task<ApiResponses<NotificationDto>> GetAllNotificationNotRead(NotificationQueryDto queryDto)
        {
            // Get list
            var notifications = await MainUnitOfWork.NotificationRepository.FindResultAsync<NotificationDto>(new Expression<Func<Notification, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.IsRead == false,
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            // Map to get CDC
            notifications.Items = await _mapperRepository.MapCreator(notifications.Items.ToList());



            return ApiResponses<NotificationDto>.Success(
                notifications.Items,
                notifications.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(notifications.TotalCount / (double)queryDto.PageSize)
            );
        }
    }

}
