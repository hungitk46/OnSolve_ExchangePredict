using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace OnSolve.ExchangeRateForecast.UnitTests.Core.Forecast
{
    public class CalculatorSlopeB
    {
        private readonly ForecastService _forecastService;
        private readonly ITestOutputHelper _output;
        private readonly List<RegressionItem> _initValue;
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
        public CalculatorSlopeB(ITestOutputHelper output)
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
        }

        [Theory]
        [InlineData(null)]
        public void ThrowsErrorWhenGivenNulInCalculatorSlope(List<RegressionItem> input)
        {
            Assert.Throws<NullReferenceException>(() => _forecastService.CalculatorSlope(input));
        }
        [Theory]
        [MemberData(nameof(_emptyListRegression))]
        public void Should_NegativeTwo_WhenGivenEmptyListRegressionInCalculatorSlope(List<RegressionItem> input)
        {
            var expected = -2;
            var result = _forecastService.CalculatorSlope(input);
            Assert.Equal(expected, result);
        }
        [Fact]
        public void CheckLinearRegresstionForeCast()
        {
            var expectedValue = 0.18784;
            var result = _forecastService.CalculatorSlope(_initValue);
            _output.WriteLine($"result: {result}");
            Assert.Equal(result, expectedValue);
        }

        bool IsEqual(double actual, double expected)
        {
            return actual == expected;
        }


    }
}
