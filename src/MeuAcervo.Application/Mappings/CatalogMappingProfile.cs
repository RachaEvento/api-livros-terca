using AutoMapper;
using MeuAcervo.Application.DTOs.Catalog;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Mappings;

public sealed class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Author, AuthorSummaryResponse>();
        CreateMap<Publisher, PublisherSummaryResponse>();
        CreateMap<BookWork, BookWorkSummaryResponse>();
        CreateMap<BookEdition, BookEditionSummaryResponse>();
        CreateMap<ExternalBookReference, ExternalBookReferenceSummaryResponse>();
    }
}
