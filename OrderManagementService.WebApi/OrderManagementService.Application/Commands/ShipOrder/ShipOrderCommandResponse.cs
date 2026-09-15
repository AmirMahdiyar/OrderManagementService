namespace OrderManagementService.Application.Commands.ShipOrder
{
    public class ShipOrderCommandResponse
    {
        public static ShipOrderCommandResponse Response() => new();
        public ShipOrderCommandResponse WithSuccess(bool success)
        {
            Success = success;
            return this;
        }
        public bool Success { get; private set; }
    }

}
