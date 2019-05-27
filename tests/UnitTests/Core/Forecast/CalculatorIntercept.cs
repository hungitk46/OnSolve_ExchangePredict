using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace OnSolve.ExchangeRateForecast.UnitTests.Core.Forecast
{
    public class CalculatorIntercept
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
                    new object[] { new List<RegressionItem>() }
                };
            }
        }
        public CalculatorIntercept(ITestOutputHelper output)
        {
            _output = output;
            _forecastService = new ForecastService();
            _initValue = new List<RegressionItem>() {
                 new RegressionItem() { X = 60 , Y = 3.1, XY = 186 , XX = 3600},
                new RegressionItem() { X = 61 , Y = 3.6, XY = 219.6, XX = 3721},
                new RegressionItem() { X = 62 , Y = 3.8, XY = 235.6 , XX = 3844},
                new RegressionItem() { X = 63 , Y = 4.0, XY = 252, XX = 3969},
                new RegressionItem() { X = 65 , Y = 4.1, XY = 266.5, XX = 4225}
            };
            forecast_date = 0;
        }

        [Theory]
        [InlineData(null)]
        public void ThrowsErrorWhenGivenNulInCalculatorIntercept(List<RegressionItem> input)
        {
            Assert.Throws<NullReferenceException>(() => _forecastService.CalculatorIntercept(input, forecast_date));
        }
        [Theory]
        [MemberData(nameof(_emptyListRegression))]
        public void Should_NaN_WhenGivenEmptyListRegressionInCalculatorIntercept(List<RegressionItem> input)
        {
            var result = _forecastService.CalculatorIntercept(input, forecast_date);
            Assert.True(double.IsNaN(result));
        }

        [Fact]
        public void CalculatorInterceptWithTemplateData()
        {
            var splope_b = 0.18784;
            var expectedValue = -7.96365;
            var result = _forecastService.CalculatorIntercept(_initValue, splope_b);
            _output.WriteLine($"result: {result}");
            Assert.True(IsEqual(result, expectedValue));
        }

        bool IsEqual(double actual, double expected)
        {
            return actual == expected;
        }


    }
}
