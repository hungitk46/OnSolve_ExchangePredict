using OnSolve.ExchangeRateForecast.Core.Entities;

namespace OnSolve.ExchangeRateForecast.Core.Specifications
{
    public sealed class RateCurrencyFromToSpecification : BaseSpecification<RateItem>
    {
        public RateCurrencyFromToSpecification(int currency_from, int currency_to)
            : base(b => b.CurrencyId == currency_from || b.CurrencyId == currency_to)
        {
            AddInclude(b=>b.CurrencyItem);
        }
    }
}
