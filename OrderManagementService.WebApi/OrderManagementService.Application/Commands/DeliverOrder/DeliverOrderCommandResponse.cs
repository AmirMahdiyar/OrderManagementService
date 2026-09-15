namespace OrderManagementService.Application.Commands.DeliverOrder
{
    public class DeliverOrderCommandResponse
    {
        public static DeliverOrderCommandResponse Response() => new();
        public DeliverOrderCommandResponse WithSuccess(bool success)
        {
            Success = success;
            return this;
        }
        public bool Success { get; set; }
    }

}
