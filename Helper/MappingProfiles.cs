using AutoMapper;

public class MappingProfiles : Profile
{
    public MappingProfiles() : base()
    {
        CreateMap<Pokemon, PokemonDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
        CreateMap<Country, CountryDto>();
        CreateMap<Owner, OwnerDto>();
        CreateMap<Review, ReviewDto>();
        CreateMap<Reviewer, ReviewerDto>();
    }
}