using ccxt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoWidget.Services
{
    public static class PriceService
    {
        private static readonly Binance _exchange = new Binance();

        public static async Task<Dictionary<string, double>> GetCryptoPricesAsync()
        {
            //await _exchange.loadMarkets();

            var symbols = new List<string> { "BTC/USDT", "ETH/USDT" };
            var ticker = await _exchange.FetchTickers(symbols);


            var prices = ticker.tickers.Values.Select(v => new { v.symbol, v.last }).ToDictionary(v => v.symbol!, v => v.last ?? 0);

            return prices;
        }
    }
}
