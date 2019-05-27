using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Interfaces;
using OnSolve.ExchangeRateForecast.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.ExchangeRateForecast.Core.Services
{
    public class ForecastService : IForecast
    {
        private readonly IAsyncRepository<CurrrencyItem> _currencyRepository;
        private readonly IAsyncRepository<RateItem> _rateRepository;
        public ForecastService() { }
        public ForecastService(IAsyncRepository<CurrrencyItem> currencyRepository, IAsyncRepository<RateItem> rateRepository)
        {
            _currencyRepository = currencyRepository;
            _rateRepository = rateRepository;
        }
        /// <summary>
        /// Forecast Rate
        /// </summary>
        /// <param name="fromCurrencyCode"></param>
        /// <param name="toCurrencyCode"></param>
        /// <param name="forrecast_data"></param>
        /// <returns>
        /// -1: Empty data to prediction
        /// -2: The dividend number of splope is 0
        /// -90: Not Exist Currency From
        /// -91: Not Exist Currency To
        /// </returns>
        public async Task<double> ForecastRate(string fromCurrencyCode, string toCurrencyCode, double forrecast_data)
        {
            var fromCurrencyObj = await _currencyRepository.ListAsync(new CurrencyGetByCodeSpecification(fromCurrencyCode));
            if (fromCurrencyObj.Count == 0) return -90; // Not Exist Currency From

            var toCurrencyIdObj = await _currencyRepository.ListAsync(new CurrencyGetByCodeSpecification(toCurrencyCode));
            if (toCurrencyIdObj.Count == 0) return -91; // Not Exist Currency To

            var historyRate = await GetHistoryRate(fromCurrencyObj.First().Id, toCurrencyIdObj.First().Id);

            var data = from rate in historyRate
                       select new RegressionItem()
                       {
                           X = rate.TimeStamp,
                           Y = rate.Rate
                       };
            var result = LinearRegresstionForeCast(data.ToList(), forrecast_data);
            return result;
        }

        async Task<List<RateItem>> GetHistoryRate(int fromCurrencyId, int toCurrencyId)
        {
            var lstRateFrom = await _rateRepository.ListAsync(new RateItemSpecification(fromCurrencyId));
            var lstRateTo = await _rateRepository.ListAsync(new RateItemSpecification(toCurrencyId));

            // Get list exchange rate CurrencyTo vs CurrencyFrom
            var data = new List<RateItem>();
            foreach (var rate in lstRateTo)
            {
                var rateFrom = lstRateFrom.Where(f => f.TimeStamp == rate.TimeStamp).First();
                if (rateFrom != null)
                {
                    if (rateFrom.Rate != 0)
                    {
                        data.Add(new RateItem()
                        {
                            CurrencyId = rate.CurrencyId,
                            TimeStamp = rate.TimeStamp,
                            Rate = rate.Rate / rateFrom.Rate
                        });
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// Prediction by LinearRegresstion medthod
        /// Regression Equation(y) = a + bx 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="forrecast_data"></param>
        /// <returns>
        /// -1: Empty data to prediction
        /// -2: The dividend number of splope is 0
        /// </returns>
        public double LinearRegresstionForeCast(List<RegressionItem> data, double forrecast_date)
        {
            // Empty data to prediction
            if (data == null || data.Count == 0) return -1;

            LabelingData(data);
            var splope_b = CalculatorSlope(data);
            var intercept_a = CalculatorInterceptA(data, splope_b);

            // Regression Equation(y) = a + bx 
            var result_forecast = intercept_a + splope_b * forrecast_date;

            return result_forecast;
        }

        /// <summary>
        /// Labeling data list. Find XY, X*X by X,Y input
        /// </summary>
        /// <param name="data"></param>
        public void LabelingData(List<RegressionItem> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                data[i].XX = data[i].X * data[i].X;
                data[i].XY = data[i].X * data[i].Y;
            }
        }

        /// <summary>
        /// Calculator substitute these values in regression equation formula
        /// Regression Equation(y) = a + bx 
        /// </summary>
        /// <param name="data">List of RegressionItem </param>
        /// <returns>
        /// -2 when dividend number is zero
        /// </returns>
        public double CalculatorSlope(List<RegressionItem> data)
        {
            return CalculatorSlopeB(data);
        }
        double CalculatorSlopeB(List<RegressionItem> data)
        {
            var N = data.Count;
            // Find ΣX, ΣY, ΣXY, ΣX2.
            var X_SUM = data.Sum(item => item.X);
            var Y_SUM = data.Sum(item => item.Y);
            var XY_SUM = data.Sum(item => item.XY);
            var XX_SUM = data.Sum(item => item.XX);

            // Substitute in the above slope formula given.
            // Slope(b) = (N*ΣXY - (ΣX)*(ΣY)) / (N*ΣX2 - (ΣX)*(ΣX))
            var divisor_number = (N * XY_SUM - X_SUM * Y_SUM);
            var dividend_number = (N * XX_SUM - X_SUM * X_SUM);
            if (dividend_number == 0) return -2;

            return Math.Round(divisor_number / dividend_number, 5);
        }

        /// <summary>
        /// Calculator Intercept by formular:
        /// Intercept(a) = (ΣY - b(ΣX)) / N 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="splope_b"></param>
        /// <returns></returns>
        public double CalculatorIntercept(List<RegressionItem> data, double splope_b)
        {
            return CalculatorInterceptA(data, splope_b);
        }
        double CalculatorInterceptA(List<RegressionItem> data, double splope_b)
        {
            var N = data.Count;
            var X_SUM = data.Sum(item => item.X);
            var Y_SUM = data.Sum(item => item.Y);
            return Math.Round((Y_SUM - splope_b * X_SUM) / N, 5);
        }

    }
}
