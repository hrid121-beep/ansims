using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public class CycleCountSchedulingService : ICycleCountSchedulingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<CycleCountSchedulingService> _logger;

        public CycleCountSchedulingService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<CycleCountSchedulingService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<CycleCountScheduleDto> CreateScheduleAsync(CycleCountScheduleDto dto)
        {
            // Implementation
            return dto;
        }
    }

}
