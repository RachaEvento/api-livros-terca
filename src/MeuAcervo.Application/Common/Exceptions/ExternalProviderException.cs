namespace MeuAcervo.Application.Common.Exceptions;

public sealed class ExternalProviderException : AppException
{
    public ExternalProviderException(string message, Exception? innerException = null)
        : base(message, "external_provider_error", innerException)
    {
    }
}
