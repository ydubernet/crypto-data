using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CryptoData.Models;
using CryptoData.ResponsesContracts;

namespace CryptoData.Services
{
    public class BinanceComputationService : IBinanceComputationService
    {
        /// <summary>
        /// Binance culture info for the API. Important for correct decimal output parsing
        /// </summary>
        private static readonly CultureInfo BINANCE_CULTURE_INFO = new CultureInfo("en-US");


        public ResultProperties ComputeHistoricalPriceFromBinance(BinanceResponseContract binanceResponseArray, int seconds, int volatilityToleranceLimitInPercent)
        {
            decimal volTolerance = volatilityToleranceLimitInPercent / 100m;

            // Parsing Binance response
            // Cf. https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md
            // timestamp in ms = binanceResponseArray[0]
            decimal opening = decimal.Parse(binanceResponseArray.Open, BINANCE_CULTURE_INFO);
            decimal high = decimal.Parse(binanceResponseArray.High, BINANCE_CULTURE_INFO);
            decimal low = decimal.Parse(binanceResponseArray.Low, BINANCE_CULTURE_INFO);
            decimal closing = decimal.Parse(binanceResponseArray.Close, BINANCE_CULTURE_INFO);

            // Unfortunately, since the only precision we can have from Binance is minute scale, we do an approximation :
            // The price at the moment the event happened is supposed to be :
            // opening + (closing - opening) * seconds / 60
            // I know how much it can be false especially during very volatil moments.
            // So I look as well as high and lows : if Highs or Lows are higher than (arbitrarily) 120% or lower than 80% of the opening / close price,
            // Or if there is more than 20% difference between opening and close
            // I then take the value which is the highest away from 20% base

            // We suppose in case the low bellow Vol tolerance limit is of an higher probability
            // than the high above Vol tolerance limit
            if (low < opening * (1 - volTolerance))
            {
                // It is then highly possible the person requesting for data at that moment
                // does so because of an ORDER_SELL_IF_LOWER_THAN transaction event
                // --> We take low value
                return new ResultProperties(low, true);
            }
            if (high > opening * (1 + volTolerance))
            {
                // It is then highly possible the person requesting for data at that moment
                // does so because of an ORDER_SELL_IF_HIGHER_THAN transaction event
                // --> We take high value
                return new ResultProperties(high, true);
            }

            return new ResultProperties(opening + (closing - opening) * seconds / 60, false);
        }
    }
}
