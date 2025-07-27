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

            try
            {
                // 設置為合約市場
                _exchange.options["defaultType"] = "swap";
                
                // 將現貨符號轉換為合約符號格式
                var contractSymbols = symbols.Select(symbol => 
                {
                    // 如果已經是合約格式（包含 :USDT），直接返回
                    if (symbol.Contains(":USDT"))
                        return symbol;
                    
                    // 將現貨符號轉換為合約符號格式
                    return $"{symbol}:USDT";
                }).ToList();

                var ticker = await _exchange.FetchTickers(contractSymbols);

                prices = ticker.tickers.Values
                    .Where(v => v.last.HasValue && v.last.Value > 0)
                    .Select(v => new { 
                        symbol = v.symbol?.Replace(":USDT", ""), // 移除合約後綴以保持與原符號一致
                        v.last 
                    })
                    .ToDictionary(v => v.symbol!, v => v.last ?? 0);
            }
            catch (System.Exception ex)
            {
                // 如果合約市場失敗，回退到現貨市場
                _exchange.options["defaultType"] = "spot";
                var ticker = await _exchange.FetchTickers(symbols);
                prices = ticker.tickers.Values
                    .Where(v => v.last.HasValue && v.last.Value > 0)
                    .Select(v => new { v.symbol, v.last })
                    .ToDictionary(v => v.symbol!, v => v.last ?? 0);
            }

            return prices;
        }
    }
}
