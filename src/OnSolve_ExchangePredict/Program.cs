using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnSolve.ExchangeRateForecast.Core.Entities;
using OnSolve.ExchangeRateForecast.Core.Interfaces;
using OnSolve.ExchangeRateForecast.Core.Services;
using OnSolve.ExchangeRateForecast.Core.Specifications;
using OnSolve.ExchangeRateForecast.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace OnSolve.ExchangeRateForecast.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region Setup DI

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<ForecastContext>(c =>
                c.UseInMemoryDatabase("PredictExchange2"))
                //c.UseSqlServer(Configuration.GetConnectionString("PredictExchangeConnection")))
                .AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>))
                .AddSingleton<IForecast, ForecastService>()
                .AddMemoryCache()
                .BuildServiceProvider();

            #endregion

            #region Seed data template

            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var forecastContext = services.GetRequiredService<ForecastContext>();
                    ForcecastContextSeed.SeedAsync(forecastContext, loggerFactory).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }

            }

            #endregion

            var _forecast = serviceProvider.GetService<IForecast>();

            Console.WriteLine("\nInput currency from:");
            var currencyFrom = Console.ReadLine().ToUpper();
            Console.WriteLine("\nInput currency to:");
            var currencyTo = Console.ReadLine().ToUpper();

            var date_forecast = Convert.ToDateTime("2017-01-15");
            var epochTime = (date_forecast.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var rate = await _forecast.ForecastRate(currencyFrom, currencyTo, epochTime);
            var msg = $"\nThe predicted currency exchange from {currencyFrom} to {currencyTo} for {date_forecast:d} is {rate}";
            if (rate < 0)
            {
                switch (rate)
                {
                    case -1:
                        msg = $"\nEmpty data to prediction";
                        break;
                    case -2:
                        msg = $"\nThe dividend number of splope is 0";
                        break;
                    case -90:
                        msg = $"\nNot Exist Currency From";
                        break;
                    case -91:
                        msg = $"\nNot Exist Currency To";
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine(msg);
            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }
}
