namespace Market.Application.Common.Exceptions;

public sealed class MarketUnauthorizedException : Exception
{
    public MarketUnauthorizedException(string message) : base(message) { }
}
