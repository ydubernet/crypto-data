namespace CryptoData.Models
{
    public class ResultProperties
    {
        public ResultProperties(decimal price, bool volatilEventHappened)
        {
            Price = price;
            VolatilEventHappened = volatilEventHappened;
        }

        public decimal Price { get; set; }
        public bool VolatilEventHappened { get; set; }
    }
}
