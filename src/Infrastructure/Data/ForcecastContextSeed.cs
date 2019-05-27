using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnSolve.ExchangeRateForecast.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.ExchangeRateForecast.Infrastructure.Data
{
    public class ForcecastContextSeed
    {
        public static async Task SeedAsync(ForecastContext forecastContext,
           ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                // TODO: Only run this if using a real database
                // context.Database.Migrate();

                forecastContext.Database.EnsureCreated();

                List<HistoricalExchangeItem> dataSeed = LoadJson(@"data\historicalExchange.json");

                if (!forecastContext.CurrrencyItems.Any())
                {
                    forecastContext.CurrrencyItems.AddRange(
                        GetPreconfiguredCurrency(dataSeed));

                    await forecastContext.SaveChangesAsync();
                }

                if (!forecastContext.RateItems.Any())
                {
                    forecastContext.RateItems.AddRange(
                        GetPreconfiguredRateItems(dataSeed));

                    await forecastContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<ForcecastContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(forecastContext, loggerFactory, retryForAvailability);
                }
            }
        }
        static IEnumerable<CurrrencyItem> GetPreconfiguredCurrency(List<HistoricalExchangeItem> lstHisotryExchange)
        {
            var lstCurrency = new List<CurrrencyItem>();
            foreach (var historyExchange in lstHisotryExchange)
            {
                var currentRates = JObject.FromObject(historyExchange.Rates);
                foreach (JProperty property in currentRates.Properties())
                {
                    var name = property.Name;
                    if (!lstCurrency.Any(x => x.Code == name))
                    {
                        lstCurrency.Add(new CurrrencyItem() { Code = name });
                    }

                }
            }
            return lstCurrency;
            //return new List<CurrrencyItem>()
            //{
            //    new CurrrencyItem() { Code = "USA"},
            //    new CurrrencyItem() { Code = "AED"},
            //    new CurrrencyItem() { Code = "AFN" },
            //    new CurrrencyItem() { Code = "ALL" },
            //    new CurrrencyItem() { Code = "AMD" },
            //    new CurrrencyItem() { Code = "ANG" },
            //    new CurrrencyItem() { Code = "VND" }
            //};
        }
        static IEnumerable<RateItem> GetPreconfiguredRateItems(List<HistoricalExchangeItem> lstHisotryExchange)
        {
            var lstCurrency = new List<CurrrencyItem>();
            var lstRates = new List<RateItem>();
            foreach (var historyExchange in lstHisotryExchange)
            {
                var currentRates = JObject.FromObject(historyExchange.Rates);
                foreach (JProperty property in currentRates.Properties())
                {
                    var name = property.Name;
                    var value = Convert.ToDouble(property.Value);
                    if (!lstCurrency.Any(x => x.Code == name))
                    {
                        lstCurrency.Add(new CurrrencyItem() { Code = name });
                    }
                    var currencyId = lstCurrency.FindIndex(x => x.Code == name) + 1;
                    lstRates.Add(new RateItem()
                    {
                        CurrencyId = currencyId,
                        Rate = value,
                        TimeStamp = historyExchange.TimeStamp
                    }); ;
                }
            }
            return lstRates;
            //return new List<RateItem>()
            //{
            //    // 2016-01-15
            //    new RateItem() { CurrencyId=1, Rate = 1, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=2, Rate = 13.672836, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=3, Rate = 68.599999, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=4, Rate = 127.5102, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=5, Rate = 484.097503, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=6, Rate = 1.7888, DateRate = Convert.ToDateTime("2016-01-15")},
            //    new RateItem() { CurrencyId=7, Rate = 22403.166667, DateRate = Convert.ToDateTime("2016-01-15")},

            //    // 2016-02-15
            //    new RateItem() { CurrencyId=1, Rate = 1, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=2, Rate = 23.672836, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=3, Rate = 68.599999, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=4, Rate = 127.5102, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=5, Rate = 484.097503, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=6, Rate = 1.7888, DateRate = Convert.ToDateTime("2016-02-15")},
            //    new RateItem() { CurrencyId=7, Rate = 23403.166667, DateRate = Convert.ToDateTime("2016-02-15")},

            //    // 2016-03-15
            //    new RateItem() { CurrencyId=1, Rate = 1, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=2, Rate = 33.672836, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=3, Rate = 68.599999, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=4, Rate = 127.5102, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=5, Rate = 484.097503, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=6, Rate = 1.7888, DateRate = Convert.ToDateTime("2016-03-15")},
            //    new RateItem() { CurrencyId=7, Rate = 24403.166667, DateRate = Convert.ToDateTime("2016-03-15")},
            //};
        }

        static List<HistoricalExchangeItem> LoadJson(string pathFile)
        {
            using (StreamReader r = new StreamReader(pathFile))
            {
                string json = r.ReadToEnd();
                List<HistoricalExchangeItem> lstHisotryExchange = JsonConvert.DeserializeObject<List<HistoricalExchangeItem>>(json);


                return lstHisotryExchange;
            }
        }
    }
    public class HistoricalExchangeItem
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public int TimeStamp { get; set; }
        public string Base { get; set; }
        public object Rates { get; set; }
    }
    public class HistoricalExchangeItemRate
    {
        public string Code { get; set; }
        public double Rate { get; set; }
    }
}
