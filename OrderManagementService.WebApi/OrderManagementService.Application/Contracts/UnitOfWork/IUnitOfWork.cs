namespace OrderManagementService.Application.Contracts.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<SavingResult> CommitAsync(CancellationToken ct = default);
    }
    public class SavingResult
    {
        public int ChangesCount { get; set; }
        public bool IsSucceeded => ChangesCount > 0;

        public void ThrowIfNoChanges<TException>() where TException : Exception, new()
        {
            if (!IsSucceeded)
                throw new TException();
        }
    }
}
