using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoData.Payloads
{
    public class BinanceParams
    {
        /// <summary>
        /// Binance Enum to retrieve Candlesticks for a given interval 
        /// </summary>
        public struct KLinesInterval
        {
            public const string OneMinute = "1m";
            public const string ThreeMinutes = "3m";
            public const string FiveMinutes = "5m";
            public const string FifteenMinutes = "15m";
            public const string ThirtyMinutes = "30m";
            public const string OneHour = "1h";
            public const string TwoHours = "2h";
            public const string FourHours = "4h";
            public const string SixHours = "6h";
            public const string EightHours = "8h";
            public const string TwelveHours = "12h";
            public const string OneDay = "1d";
            public const string ThreeDays = "3d";
            public const string OneWeek = "1w";
            public const string OneMonth = "1M";
        }

        /// <summary>
        /// Binance params for KLines endpoint
        /// </summary>
        public class KLinesParams
        {
            public const string Symbol = "symbol"; // mandatory
            public const string Interval = "interval"; // mandatory ; enum
            public const string StartTime = "startTime"; // optional
            public const string EndTime = "endTime"; // optional
            public const string Limit = "limit"; // optional
        }
    }
}
