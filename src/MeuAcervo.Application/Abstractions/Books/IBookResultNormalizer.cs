namespace MeuAcervo.Application.Abstractions.Books;

public interface IBookResultNormalizer<in TSource, out TResult>
{
    TResult Normalize(TSource source);
}
