using MedicalERP.Application.Reports.Dtos;

namespace MedicalERP.Application.Interfaces;

public interface IReportService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<SalesReportDto> GetSalesReportAsync(SalesReportFilterDto filter, CancellationToken cancellationToken = default);
}
