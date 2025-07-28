using ccxt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoWidget.Services
{
    public static class PriceService
    {
        public static async Task<Dictionary<string, double?>> GetCryptoPricesAsync(List<string> symbols, string exchangeName = "binance")
        {
            var prices = new Dictionary<string, double?>();
            if (symbols == null || symbols.Count == 0)
                return prices;

            try
            {
                // 動態創建交易所實例
                var exchange = ExchangeUtil.Create(exchangeName);
                if (exchange == null)
                {
                    // 如果創建失敗，回退到 Binance
                    exchange = new Binance();
                }

                // 逐個處理每個符號
                foreach (var symbol in symbols)
                {
                    double? price = null;

                    try
                    {
                        // 嘗試合約市場
                        exchange.options["defaultType"] = "swap";

                        string contractSymbol;
                        if (symbol.Contains(":USDT"))
                        {
                            contractSymbol = symbol;
                        }
                        else
                        {
                            contractSymbol = $"{symbol}:USDT";
                        }

                        var ticker = await exchange.FetchTicker(contractSymbol);
                        if (ticker.last.HasValue && ticker.last.Value > 0)
                        {
                            price = ticker.last.Value;
                        }
                    }
                    catch (System.Exception)
                    {
                        try
                        {
                            // 如果合約市場失敗，嘗試現貨市場
                            exchange.options["defaultType"] = "spot";
                            var ticker = await exchange.FetchTicker(symbol);
                            if (ticker.last.HasValue && ticker.last.Value > 0)
                            {
                                price = ticker.last.Value;
                            }
                        }
                        catch (System.Exception)
                        {
                            // 如果現貨市場也失敗，價格保持為 null
                            price = null;
                        }
                    }

                    // 將結果加入字典
                    prices[symbol] = price;
                }
            }
            catch (System.Exception)
            {
                // 如果交易所創建失敗，所有價格設為 null
                foreach (var symbol in symbols)
                {
                    prices[symbol] = null;
                }
            }

            return prices;
        }
    }
}
