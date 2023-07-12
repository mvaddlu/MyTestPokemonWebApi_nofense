namespace PokemonApi.Repositories;

public interface ICountryRepository
{
    ICollection<Country> GetCountries();
    Country? GetCountry(int countryId);
    Country? GetCountryByOwner(int ownerId);
    ICollection<Owner>? GetOwners(int countryId);
    bool CountryExists(int countryId);
    bool CreateCountry(Country country);
    bool UpdateCountry(Country country);
    bool DeleteCountry(Country country);
    bool Save();
}