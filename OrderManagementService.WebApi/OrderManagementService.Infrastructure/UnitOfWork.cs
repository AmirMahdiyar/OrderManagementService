using OrderManagementService.Application.Contracts.UnitOfWork;

namespace OrderManagementService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderManagementDbContext _context;

        public UnitOfWork(OrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<SavingResult> CommitAsync(CancellationToken ct = default)
        {
            var savedChangedStateCount = await _context.SaveChangesAsync(ct);
            return new SavingResult { ChangesCount = savedChangedStateCount };
        }
    }
}

