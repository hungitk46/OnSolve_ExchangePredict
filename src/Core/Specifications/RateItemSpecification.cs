using OnSolve.ExchangeRateForecast.Core.Entities;

namespace OnSolve.ExchangeRateForecast.Core.Specifications
{
    public sealed class RateItemSpecification : BaseSpecification<RateItem>
    {
        /// <summary>
        /// Get List Rate By CurrencyId
        /// </summary>
        /// <param name="id"></param>
        public RateItemSpecification(int id)
            : base(b => b.CurrencyId == id)
        {
            AddInclude(b => b.CurrencyItem);
        }
        /// <summary>
        /// Get List Rate using combine currency from and currency to
        /// </summary>
        /// <param name="currency_from"></param>
        /// <param name="currency_to"></param>
        public RateItemSpecification(int currency_from, int currency_to)
           : base(b => b.CurrencyId == currency_from || b.CurrencyId == currency_to)
        {
            AddInclude(b => b.CurrencyItem);
        }
    }
}
