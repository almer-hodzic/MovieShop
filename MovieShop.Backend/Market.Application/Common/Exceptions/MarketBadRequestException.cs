namespace Market.Application.Common.Exceptions;

public sealed class MarketBadRequestException : Exception
{
    public MarketBadRequestException(string message) : base(message) { }
}
