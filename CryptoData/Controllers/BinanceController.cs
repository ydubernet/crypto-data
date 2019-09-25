using CryptoData.Payloads;
using CryptoData.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceController : ControllerBase
    {
        private static HttpClient _httpClient = new HttpClient();

        private const string BINANCE_API_BASE = "https://api.binance.com";

        /// <summary>
        /// Tells Binance to return us max 1 raw of data for the request
        /// </summary>
        /// 
        /// <remarks>
        /// Binance does not allow more than minute scale of precision
        /// </remarks>
        private const int LIMIT = 1;

        /// <summary>
        /// Arbitrary value used for avoiding taking wrong prices in the model in case of very volatil event
        /// </summary>
        public const decimal VOLATILITY_TOLERANCE_LIMIT = 0.2m;

        private readonly IBinanceComputationService _binanceComputationService;

        public BinanceController(IBinanceComputationService binanceComputationService)
        {
            _binanceComputationService = binanceComputationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Market symbol (here Binance one) we want to retrieve data from</param>
        /// <param name="timestamp">Unix timestamp of when the event occured</param>
        /// <param name="volLimit">in percent. used for avoiding taking wrong prices in the model in case of very volatil event</param>
        /// <returns></returns>
        [HttpGet]
        [Route("price")]
        public async Task<IActionResult> GetCryptoCurrencyPrice([FromQuery]string symbol, [FromQuery]long timestamp, [FromQuery]int volLimit)
        {
            long timestampInMs;

            // when timestamp is something else than the exact minute the event appeared (for instance 2019-09-08 19:34:23),
            // Binance returns the ticker starting the minute just AFTER the event (for instance 2019-09-08 19:35:00).
            // In order to be within the minute, we need to withraw the seconds part.

            int seconds = (int)timestamp % 60; // Yup, whatever % 60 will definitely be ok to be stored in an int ;)
            timestampInMs = (timestamp - seconds) * 1000;// Binance takes timestamp in ms.

            var responseContent = await _httpClient.GetAsync(BINANCE_API_BASE + $"/api/v1/klines?{BinanceParams.KLinesParams.Symbol}={symbol}&{BinanceParams.KLinesParams.StartTime}={timestampInMs}&{BinanceParams.KLinesParams.Interval}={BinanceParams.KLinesInterval.OneMinute}&{BinanceParams.KLinesParams.Limit}={LIMIT}");

            if(!responseContent.IsSuccessStatusCode)
            {
                return BadRequest(responseContent.StatusCode + ": Binance call failed, you miss mandatory params such as symbol and timestamp");
            }

            string responseMessage = await responseContent.Content.ReadAsStringAsync();

            if (!responseMessage.StartsWith("["))
            {
                return BadRequest("Binance response isn't the one expected. Please kindly notify the developer so he adapts the code.");
            }

            // TODO : Stop using Newtonsoft and use Jit instead
            string[][] response = JsonConvert.DeserializeObject<string[][]>(responseMessage);
            string[] responseArray = response[0];

            if(responseArray.Length != 12)
            {
                return BadRequest("Binance response doesn't contain the expected number of elements. API may have changed. Please kindly notify the developer so he adapts the code.");
            }

            var result = _binanceComputationService.ComputeHistoricalPriceFromBinance(responseArray, seconds, volLimit);
            return Ok(result);
        }
    }
}
