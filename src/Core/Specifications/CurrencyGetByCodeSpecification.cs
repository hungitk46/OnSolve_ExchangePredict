using OnSolve.ExchangeRateForecast.Core.Entities;

namespace OnSolve.ExchangeRateForecast.Core.Specifications
{
    public sealed class CurrencyGetByCodeSpecification : BaseSpecification<CurrrencyItem>
    {
        public CurrencyGetByCodeSpecification(string code)
            : base(b => b.Code == code)
        {
        }
    }
}
