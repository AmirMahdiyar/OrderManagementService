namespace OrderManagementService.Application.Commands.BulkInsertOrders
{
    public record BulkInsertOrdersCommandResponse
    {
        public int InsertedCount { get; private set; }

        public static BulkInsertOrdersCommandResponse Response() => new();

        public BulkInsertOrdersCommandResponse WithInsertedCount(int count)
        {
            InsertedCount = count;
            return this;
        }
    }

}
