using CryptoData.Models;
using CryptoData.ResponsesContracts;

namespace CryptoData.Services
{
    public interface IBinanceComputationService
    {
        ResultProperties ComputeHistoricalPriceFromBinance(BinanceResponseContract binanceResponseArray, int seconds, int volatilityToleranceLimitInPercent);
    }
}