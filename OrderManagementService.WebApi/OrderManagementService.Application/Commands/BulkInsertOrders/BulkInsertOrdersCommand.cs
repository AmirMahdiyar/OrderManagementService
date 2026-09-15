using OrderManagementService.Application.Base;
using OrderManagementService.Application.Dtos;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.BulkInsertOrders
{

    public class BulkInsertOrdersCommand : CommandBase<BulkInsertOrdersCommandResponse>
    {
        public List<BulkOrderDto> Orders { get; set; } = new();
        public override void Validate()
        {
            new BulkInsertOrdersCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
