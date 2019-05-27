using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Services;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace OnSolve.ExchangeRateForecast.UnitTests.Core.Forecast
{
    public class LinearRegressionDataLabeling
    {
        private readonly ForecastService _forecastService;
        private readonly List<RegressionItem> _initValue;

        public LinearRegressionDataLabeling(ITestOutputHelper output)
        {
            _forecastService = new ForecastService();
            _initValue = new List<RegressionItem>() {
                new RegressionItem() { X = 60 , Y = 3.1 },
                new RegressionItem() { X = 61 , Y = 3.6 },
                new RegressionItem() { X = 62 , Y = 3.8 },
                new RegressionItem() { X = 63 , Y = 4.0 },
                new RegressionItem() { X = 65 , Y = 4.1 }
            };
        }

        [Fact]
        public void CheckLabelingDataInputLinearRegression()
        {
            var expectedValue = new List<RegressionItem>() {
                new RegressionItem() { X = 60 , Y = 3.1, XY = 186 , XX = 3600},
                new RegressionItem() { X = 61 , Y = 3.6, XY = 219.6, XX = 3721},
                new RegressionItem() { X = 62 , Y = 3.8, XY = 235.6 , XX = 3844},
                new RegressionItem() { X = 63 , Y = 4.0, XY = 252, XX = 3969},
                new RegressionItem() { X = 65 , Y = 4.1, XY = 266.5, XX = 4225}
            };
            _forecastService.LabelingData(_initValue);

            Assert.True(CompareTwoListRegresstionItems(_initValue, expectedValue));

        }

        bool CompareTwoListRegresstionItems(List<RegressionItem> list1, List<RegressionItem> list2)
        {
            while (list1.Count != list2.Count) return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if ((list1[i].X != list2[i].X) || (list1[i].Y != list2[i].Y) || (list1[i].XY != list2[i].XY)
                        || (list1[i].XX != list2[i].XX))
                    return false;
            }
            return true;
        }
    }
}
