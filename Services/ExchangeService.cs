using ccxt;
using CryptoWidget.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoWidget.Services
{
    public static class ExchangeService
    {
        private static Exchange? _sharedExchange;
        private static string? _currentExchangeName;
        private static string? _currentApiKey;
        private static string? _currentApiSecret;

        private static Exchange EnsureExchange(string exchangeName)
        {
            if (_sharedExchange == null || !string.Equals(_currentExchangeName, exchangeName, StringComparison.OrdinalIgnoreCase))
            {
                var exchange = ExchangeUtil.Create(exchangeName);
                if (exchange == null)
                {
                    exchange = new Binance();
                }

                _sharedExchange = exchange;
                _currentExchangeName = exchangeName;
                _currentApiKey = null;
                _currentApiSecret = null;
            }

            return _sharedExchange;
        }

        private static Exchange EnsureExchangeWithAuth(string exchangeName, string apiKey, string? apiSecret)
        {
            // 先確保交易所類型正確，再檢查憑證是否需更新
            var exchange = EnsureExchange(exchangeName);

            if (_currentApiKey != apiKey || _currentApiSecret != apiSecret)
            {
                exchange.apiKey = apiKey;
                exchange.secret = apiSecret ?? string.Empty;
                _currentApiKey = apiKey;
                _currentApiSecret = apiSecret;
            }

            return exchange;
        }

        public static async Task<Dictionary<string, double?>> GetCryptoPricesAsync(List<string> symbols, string exchangeName = "binance")
        {
            var prices = new Dictionary<string, double?>();
            if (symbols == null || symbols.Count == 0)
                return prices;

            try
            {
                var exchange = EnsureExchange(exchangeName);

                foreach (var symbol in symbols)
                {
                    double? price = null;

                    bool isContractFormat = symbol.Contains(":USDT");
                    bool isSpotFormat = symbol.Contains("/USDT");

                    if (isContractFormat)
                    {
                        price = await TryGetPriceAsync(exchange, symbol, true);
                        if (price == null)
                        {
                            string spotSymbol = symbol.Replace(":", "/");
                            price = await TryGetPriceAsync(exchange, spotSymbol, false);
                        }
                    }
                    else
                    {
                        string spotSymbol = isSpotFormat ? symbol : $"{symbol}/USDT";
                        price = await TryGetPriceAsync(exchange, spotSymbol, false);
                        if (price == null)
                        {
                            string contractSymbol = symbol.Replace("/", ":");
                            price = await TryGetPriceAsync(exchange, contractSymbol, true);
                        }
                    }

                    prices[symbol] = price;
                }
            }
            catch (Exception)
            {
                foreach (var symbol in symbols)
                {
                    prices[symbol] = null;
                }
            }

            return prices;
        }

        private static async Task<double?> TryGetPriceAsync(Exchange exchange, string symbol, bool isContract)
        {
            try
            {
                exchange.options["defaultType"] = isContract ? "swap" : "spot";

                var ticker = await exchange.FetchTicker(symbol);
                if (ticker.last.HasValue && ticker.last.Value > 0)
                {
                    return ticker.last.Value;
                }
            }
            catch (Exception)
            {
                // ignore and return null
            }

            return null;
        }

        public static async Task<List<PositionItem>> GetOpenPositionsAsync(string exchangeName, string apiKey, string? apiSecret = null)
        {
            var result = new List<PositionItem>();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return result; // 無金鑰則直接回傳空集合
            }

            var exchange = EnsureExchangeWithAuth(exchangeName, apiKey, apiSecret);

            try
            {
                // 嘗試呼叫統一 API 取得持倉（不同交易所支援度不同）
                await exchange.LoadMarkets();

                List<Position> positions = await exchange.FetchPositions();

                foreach (Position p in positions)
                {
                    try
                    {
                        // 盡可能穩健地讀取欄位
                        string symbol = p.symbol;
                        string? side = p.side;
                        double? contracts = p.contracts;
                        double? entryPrice = p.entryPrice;
                        double? upnl = p.unrealizedPnl;

                        bool isContractFormat = symbol.Contains(":USDT");
                        if (isContractFormat)
                        {
                            symbol = symbol.Replace("/USDT", "");
                        }

                        // 僅加入有部位的資料（contracts > 0）
                        if ((contracts ?? 0) > 0 || (side != null && !string.Equals(side, "flat", StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new PositionItem
                            {
                                Symbol = symbol,
                                Side = side,
                                Contracts = contracts,
                                EntryPrice = entryPrice,
                                UnrealizedPnl = upnl
                            });
                        }
                    }
                    catch { /* 單筆解析失敗忽略 */ }
                }
            }
            catch
            {
                // 某些交易所或現貨帳戶不支援持倉查詢，回傳空集合
            }

            return result;
        }
    }
}


