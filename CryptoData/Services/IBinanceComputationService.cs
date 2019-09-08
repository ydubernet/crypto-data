using CryptoData.Models;

namespace CryptoData.Services
{
    public interface IBinanceComputationService
    {
        ResultProperties ComputeHistoricalPriceFromBinance(string[] binanceResponseArray, int seconds, int volatilityToleranceLimitInPercent);
    }
}