using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Services;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace OnSolve.ExchangeRateForecast.UnitTests.Core.Forecast
{
    public class LinearRegresstionForeCast
    {
        private readonly ForecastService _forecastService;
        private readonly ITestOutputHelper _output;
        private readonly List<RegressionItem> _initValue;
        private readonly double forecast_date;
        public static IEnumerable<object[]> _emptyListRegression
        {
            get
            {
                return new[] {
                    new object[] { new List<RegressionItem>() },
                    new object[] { new List<RegressionItem>() },
                };
            }
        }
        public LinearRegresstionForeCast(ITestOutputHelper output)
        {
            _output = output;
            _forecastService = new ForecastService();
            _initValue = new List<RegressionItem>() {
                new RegressionItem() { X = 60 , Y = 3.1 },
                new RegressionItem() { X = 61 , Y = 3.6 },
                new RegressionItem() { X = 62 , Y = 3.8 },
                new RegressionItem() { X = 63 , Y = 4.0 },
                new RegressionItem() { X = 65 , Y = 4.1 }
            };
            forecast_date = 0;
        }


        [Theory]
        [InlineData(null)]
        [MemberData(nameof(_emptyListRegression))]
        public void Should_ReturnNegativeOneIfDataInputIsNullOrEmpty_LinearRegresstionForeCast(List<RegressionItem> input)
        {
            
            var actual = _forecastService.LinearRegresstionForeCast(input, forecast_date);
            Assert.Equal(-1, actual);
        }

        [Fact]
        public void CheckLinearRegresstionForeCast()
        {
            var expectedValue = 0.05981;
            var result = _forecastService.CalculatorSlope(_initValue);
            _output.WriteLine($"result: {result}");

            Assert.Equal(expectedValue, result);
        }


    }
}
