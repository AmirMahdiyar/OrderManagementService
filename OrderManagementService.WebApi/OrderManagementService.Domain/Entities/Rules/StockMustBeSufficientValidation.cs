using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class StockMustBeSufficientValidation : DomainValidation<Quantity, InsufficientStockException>
    {
        private readonly Quantity _amountToDecrease;
        public StockMustBeSufficientValidation(Quantity stock, Quantity amountToDecrease) : base(stock)
        {
            _amountToDecrease = amountToDecrease;
        }
        protected override bool IsValid() => Value.Value >= _amountToDecrease.Value;
    }
}
