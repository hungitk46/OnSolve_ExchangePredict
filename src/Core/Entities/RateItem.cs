namespace OnSolve.ExchangeRateForecast.Core.Entities
{
    public class RateItem: BaseEntity
    {
        public int CurrencyId { get; set; }
        public CurrrencyItem CurrencyItem { get; set; }
        public double TimeStamp { get; set; }
        public double Rate { get; set; }

    }
}
