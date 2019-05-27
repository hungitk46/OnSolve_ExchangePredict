using System.Threading.Tasks;

namespace OnSolve.ExchangeRateForecast.Core.Interfaces
{
    public interface IForecast
    {
        Task<double> ForecastRate(string fromCurrency, string toCurrency, double forrecast_data);
    }
}
