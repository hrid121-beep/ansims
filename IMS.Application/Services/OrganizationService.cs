using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationStatsDto> GetOrganizationStatsAsync()
        {
            var ranges = await _unitOfWork.Ranges.CountAsync(r => r.IsActive);
            var battalions = await _unitOfWork.Battalions.CountAsync(b => b.IsActive);
            var zilas = await _unitOfWork.Zilas.CountAsync(z => z.IsActive);
            var upazilas = await _unitOfWork.Upazilas.CountAsync(u => u.IsActive);

            return new OrganizationStatsDto
            {
                RangeCount = ranges,
                BattalionCount = battalions,
                ZilaCount = zilas,
                UpazilaCount = upazilas
            };
        }
    }
}
