namespace OrderManagementService.Application.Contracts.Repositories
{
    public interface ICustomerCommandRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(List<Guid> ids, CancellationToken ct = default);
    }
}
