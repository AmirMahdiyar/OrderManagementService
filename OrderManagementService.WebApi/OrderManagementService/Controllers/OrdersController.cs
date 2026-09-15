using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Application.Commands.BulkInsertOrders;
using OrderManagementService.Application.Commands.ConfirmOrder;
using OrderManagementService.Application.Commands.CreateOrder;
using OrderManagementService.Application.Commands.DeleteOrder;
using OrderManagementService.Application.Commands.DeliverOrder;
using OrderManagementService.Application.Commands.ShipOrder;
using OrderManagementService.Application.Queries.GetFilteredOrders;
using OrderManagementService.Application.Queries.GetOrderById;

namespace OrderManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFilteredOrders([FromQuery] GetFilteredOrdersQuery query, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(query, cancellationToken));


        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetOrderByIdQuery { OrderId = id }, cancellationToken));


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetOrderById), new { id = response.OrderId }, response);
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkInsertOrders([FromBody] BulkInsertOrdersCommand command, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(command, cancellationToken));

        [HttpPost("{id:guid}/confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder([FromRoute] Guid id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new ConfirmOrderCommand { OrderId = id }, cancellationToken));


        [HttpPost("{id:guid}/ship")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShipOrder([FromRoute] Guid id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new ShipOrderCommand { OrderId = id }, cancellationToken));


        [HttpPost("{id:guid}/deliver")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeliverOrder([FromRoute] Guid id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeliverOrderCommand { OrderId = id }, cancellationToken));

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteOrderCommand { OrderId = id }, cancellationToken));

    }
}
