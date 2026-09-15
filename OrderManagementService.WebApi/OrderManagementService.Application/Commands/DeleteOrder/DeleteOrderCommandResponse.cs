namespace OrderManagementService.Application.Commands.DeleteOrder
{
    public record DeleteOrderCommandResponse
    {
        public bool Success { get; private set; }

        private DeleteOrderCommandResponse() { }

        public static DeleteOrderCommandResponse Response() => new DeleteOrderCommandResponse();

        public DeleteOrderCommandResponse WithSuccess(bool success)
        {
            Success = success;
            return this;
        }
    }

}
