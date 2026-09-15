using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.Services.DomainServices.OrderConfirmation
{
    public interface IOrderConfirmationDomainService
    {
        Task ConfirmAsync(Order order, CancellationToken cancellationToken = default);
    }
}
