using ccxt;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CryptoWidget.Services
{
    public static class ExchangeUtil
    {
        private static readonly Assembly _ccxtAsm = typeof(Exchange).Assembly;

        public static Exchange Create(string id, object? config = null)
        {
            // ccxt 的類別命名 = exchange id，小寫（binance、bybit...）
            var type = _ccxtAsm.GetType($"ccxt.{id}", throwOnError: true, ignoreCase: true);
            var ex = (Exchange)Activator.CreateInstance(type!, config)!;

            return ex;
        }

        public static List<string> GetExchanges()
        {
            var tmp = new ccxt.binance();
            return ccxt.Exchange.exchanges;
        }
    }
}
