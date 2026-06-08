namespace MeuAcervo.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message, string errorCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
