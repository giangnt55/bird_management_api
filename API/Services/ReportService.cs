using System.Linq.Expressions;
using API.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API.Services;

public interface IReportService : IBaseService
{
    Task<ApiResponse> CreateReport(ReportCreateDto reportCreateDto);
    
    Task<ApiResponse> CancelReport(ReportCreateDto reportCreateDto);
}

public class ReportService : BaseService, IReportService
{
    public ReportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> CreateReport(ReportCreateDto reportCreateDto)
    {
        var exist = await MainUnitOfWork.ReportRepository.FindOneAsync(new Expression<Func<Report, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.CreatorId == AccountId,
            x => (x.PostId != null && x.PostId == reportCreateDto.PostId) ||
                 (x.CommentId != null && x.CommentId == reportCreateDto.CommentId)
        });

        if (exist != null)
            throw new ApiException("You already report this content", StatusCode.BAD_REQUEST);

        var existPost = await MainUnitOfWork.PostRepository.FindOneAsync(new Expression<Func<Post, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x =>  reportCreateDto.PostId != null && x.Id == reportCreateDto.PostId
        });

        var report = reportCreateDto.ProjectTo<ReportCreateDto, Report>();

        if (existPost == null)
        {
            report.PostId = null;
        }
        else
        {
            report.CommentId = null;
        }

        if (!await MainUnitOfWork.ReportRepository.InsertAsync(report, AccountId, CurrentDate))
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> CancelReport(ReportCreateDto reportCreateDto)
    {
        var exist = await MainUnitOfWork.ReportRepository.FindOneAsync(new Expression<Func<Report, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.CreatorId == AccountId,
            x => (x.PostId != null && x.PostId == reportCreateDto.PostId) ||
                 (x.CommentId != null && x.CommentId == reportCreateDto.CommentId)
        });

        if (exist == null)
            throw new ApiException(MessageKey.NotFound, StatusCode.BAD_REQUEST);

        if (!await MainUnitOfWork.ReportRepository.DeleteAsync(exist, AccountId, CurrentDate))
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success();
    }
}