using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnSolve.ExchangeRateForecast.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnSolve.ExchangeRateForecast.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IForecast _forecast;
        private readonly IMemoryCache _cache;
        private readonly string _itemsKeyTemplate = "forecast-{0}-{1}-{2}";// currencyFrom-currencyTo-epochTime-
        private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);
        public ForecastController(IMemoryCache cache, IForecast forecast)
        {
            _forecast = forecast;
            _cache = cache;
        }
        // GET api/forecast
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAsync()
        {
            var date_forecast = Convert.ToDateTime("2017-01-15");
            var currencyFrom = "USD";
            var currencyTo = "VND";
            var epochTime = (date_forecast.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var cacheKey = string.Format(_itemsKeyTemplate, currencyFrom, currencyTo, epochTime);
            //rate = await _forecast.ForecastRate(currencyFrom, currencyTo, epochTime);
            double rate = 0;
            rate = await _cache.GetOrCreate(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _forecast.ForecastRate(currencyFrom, currencyTo, epochTime);
            });
            return new string[] { rate.ToString() };
        }
    }
}
