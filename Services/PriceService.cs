using ccxt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoWidget.Services
{
    public static class PriceService
    {
        private static readonly Binance _exchange = new Binance();

        public static async Task<Dictionary<string, double>> GetCryptoPricesAsync(List<string> symbols)
        {
            var prices = new Dictionary<string, double>();
            if (symbols == null || symbols.Count == 0)
                return prices;
            var ticker = await _exchange.FetchTickers(symbols);

            prices = ticker.tickers.Values.Select(v => new { v.symbol, v.last }).ToDictionary(v => v.symbol!, v => v.last ?? 0);

            return prices;
        }
    }
}
