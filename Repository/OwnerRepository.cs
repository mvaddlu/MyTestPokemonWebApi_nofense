using AutoMapper;

public class OwnerRepository : IOwnerRepository 
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public OwnerRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public Owner? GetOwner(int ownerId) 
    {
        return _context.Owners.FirstOrDefault(o => o.Id == ownerId);
    }

    public Owner? GetOwnerOfAPokemon(int pokemonId) 
    {
        return _context.PokemonOwners
            .Where(po => po.PokemonId == pokemonId)
            .Select(po => po.Owner) // Для підвантаження овнерів
            .FirstOrDefault();
    }

    public ICollection<Owner> GetOwners() 
    {
        return _context.Owners.ToList();
    }

    public ICollection<Pokemon> GetPokemonsByOwner(int ownerId) 
    {
        return _context.PokemonOwners
            .Where(po => po.OwnerId == ownerId)
            .Select(po => po.Pokemon)
            .ToList();
    }

    public bool OwnerExists(int ownerId) 
    {
        return _context.Owners.Any(o => o.Id == ownerId);
    }
}