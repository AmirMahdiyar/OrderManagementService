namespace OrderManagementService.Application.Commands.ConfirmOrder
{
    public record ConfirmOrderCommandResponse
    {
        public bool Success { get; private set; }

        private ConfirmOrderCommandResponse() { }

        public static ConfirmOrderCommandResponse Response() => new ConfirmOrderCommandResponse();

        public ConfirmOrderCommandResponse WithSuccess(bool success)
        {
            Success = success;
            return this;
        }
    }

}
