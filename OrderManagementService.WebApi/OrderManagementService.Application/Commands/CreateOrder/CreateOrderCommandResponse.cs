namespace OrderManagementService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandResponse
    {
        public static CreateOrderCommandResponse Response() => new();

        public CreateOrderCommandResponse Succeeded()
        {
            Success = true;
            return this;
        }
        public CreateOrderCommandResponse WithOrderId(Guid id)
        {
            OrderId = id;
            return this;
        }

        public Guid OrderId { get; set; }
        public bool Success { get; private set; }

    }

}
