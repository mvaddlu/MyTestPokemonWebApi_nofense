using AutoMapper;

public class MappingProfiles : Profile
{
    public MappingProfiles() : base()
    {
        CreateMap<Pokemon, PokemonDto>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<Owner, OwnerDto>().ReverseMap();
        CreateMap<Review, ReviewDto>().ReverseMap();
        CreateMap<Reviewer, ReviewerDto>().ReverseMap();
    }
}