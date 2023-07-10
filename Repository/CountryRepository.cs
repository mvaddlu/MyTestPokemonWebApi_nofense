using AutoMapper;

public class CountryRepository : ICountryRepository 
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public CountryRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public bool CountryExists(int countryId) 
    {
        return _context.Countries.Any(c => c.Id == countryId);
    }

    public ICollection<Country> GetCountries() 
    {
        return _context.Countries.ToList();
    }

    public Country? GetCountry(int countryId) 
    {
        return _context.Countries.FirstOrDefault(c => c.Id == countryId);
    }

    public Country? GetCountryByOwner(int ownerId) 
    {
        // Йдемо таким способом тому, що DataContext не включає дані по типу країн неявно
        // на це потрібно явно вказати
        return _context.Owners.Where(o => o.Id == ownerId).Select(o => o.Country).FirstOrDefault();
    }

    public ICollection<Owner>? GetOwners(int countryId) 
    {
        return _context.Owners.Where(o => o.Country.Id == countryId).ToList();
    }

    public bool CreateCountry(Country country)
    {
        country.Id = 0; // IDENTITY_INSERT=OFF
        _context.Add(country);
        return Save();
    }
    public bool Save()
    {
        var saveChanges = _context.SaveChanges();
        return saveChanges > 0 ? true : false; 
    }
}