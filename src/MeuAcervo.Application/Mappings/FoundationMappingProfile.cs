using AutoMapper;
using MeuAcervo.Application.DTOs.Identity;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Mappings;

public sealed class FoundationMappingProfile : Profile
{
    public FoundationMappingProfile()
    {
        CreateMap<Tenant, TenantSummaryResponse>();
        CreateMap<User, UserSummaryResponse>();
    }
}
