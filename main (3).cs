using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockPulse
{
    public class Stock
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class StockService
    {
        private readonly Random random = new Random();

        public async Task<Stock> FetchStockDataAsync(string symbol)
        {
            await Task.Delay(random.Next(500, 1500));

            decimal price = Math.Round((decimal)(random.NextDouble() * 500 + 50), 2);
            decimal changePercent = Math.Round((decimal)(random.NextDouble() * 10 - 5), 2);

            return new Stock
            {
                Symbol = symbol.ToUpper(),
                Price = price,
                ChangePercent = changePercent,
                LastUpdated = DateTime.Now
            };
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            StockService stockService = new StockService();

            List<string> stockSymbols = new List<string>
            {
                "AAPL",
                "MSFT",
                "TSLA",
                "AMZN",
                "GOOG",
                "NVDA",
                "META"
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine(" Team StockPulse Stock Monitor");
                Console.WriteLine("====================================");
                Console.WriteLine("Fetching real-time stock data...\n");

                List<Task<Stock>> stockTasks = stockSymbols
                    .Select(symbol => stockService.FetchStockDataAsync(symbol))
                    .ToList();

                Stock[] stocks = await Task.WhenAll(stockTasks);

                var sortedStocks = stocks
                    .OrderByDescending(stock => stock.ChangePercent)
                    .ToList();

                Console.WriteLine("Symbol\tPrice\t\tChange %\tLast Updated");
                Console.WriteLine("-------------------------------------------------------");

                foreach (var stock in sortedStocks)
                {
                    Console.WriteLine($"{stock.Symbol}\t${stock.Price}\t\t{stock.ChangePercent}%\t\t{stock.LastUpdated:T}");
                }

                var topGainer = stocks.OrderByDescending(stock => stock.ChangePercent).First();
                var topLoser = stocks.OrderBy(stock => stock.ChangePercent).First();

                Console.WriteLine("\nTop Gainer:");
                Console.WriteLine($"{topGainer.Symbol} increased by {topGainer.ChangePercent}%");

                Console.WriteLine("\nTop Loser:");
                Console.WriteLine($"{topLoser.Symbol} decreased by {topLoser.ChangePercent}%");

                Console.WriteLine("\nPress Q to quit or any other key to refresh.");
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Q)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine("\nStockPulse monitor closed.");
        }
    }
}