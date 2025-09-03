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

                    // 判斷幣種格式並選擇優先市場
                    bool isContractFormat = symbol.Contains(":USDT");
                    bool isSpotFormat = symbol.Contains("/USDT");

                    if (isContractFormat)
                    {
                        // 合約格式：優先嘗試合約市場，失敗時回退到現貨市場
                        price = await TryGetPriceAsync(exchange, symbol, true);
                        if (price == null)
                        {
                            // 合約市場失敗，嘗試現貨市場
                            string spotSymbol = symbol.Replace(":", "/");
                            price = await TryGetPriceAsync(exchange, spotSymbol, false);
                        }
                    }
                    else
                    {
                        // 簡短格式或完整格式：優先嘗試現貨市場，失敗時嘗試合約市場
                        string spotSymbol = isSpotFormat ? symbol : $"{symbol}/USDT";
                        price = await TryGetPriceAsync(exchange, spotSymbol, false);
                        if (price == null)
                        {
                            // 現貨市場失敗，嘗試合約市場
                            string contractSymbol = symbol.Replace("/", ":");
                            price = await TryGetPriceAsync(exchange, contractSymbol, true);
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

        /// <summary>
        /// 嘗試從指定市場獲取價格
        /// </summary>
        /// <param name="exchange">交易所實例</param>
        /// <param name="symbol">幣種符號</param>
        /// <param name="isContract">是否為合約市場</param>
        /// <returns>價格，失敗時返回 null</returns>
        private static async Task<double?> TryGetPriceAsync(Exchange exchange, string symbol, bool isContract)
        {
            try
            {
                // 設定市場類型
                exchange.options["defaultType"] = isContract ? "swap" : "spot";

                var ticker = await exchange.FetchTicker(symbol);
                if (ticker.last.HasValue && ticker.last.Value > 0)
                {
                    return ticker.last.Value;
                }
            }
            catch (System.Exception)
            {
                // 價格獲取失敗，返回 null
            }

            return null;
        }
    }
}
