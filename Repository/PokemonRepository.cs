public class PokemonRepository : IPokemonRepository 
{
    private readonly DataContext _context;
    public PokemonRepository(DataContext context)
    {
        _context = context;
    }
    public ICollection<Pokemon> GetPokemons() 
    {
        return _context.Pokemons.OrderBy(p => p.Id).ToList();
    }

    public Pokemon GetPokemon(int id) 
    {
        return _context.Pokemons.FirstOrDefault(p => p.Id == id);
    }

    public Pokemon GetPokemon(string name) 
    {
        return _context.Pokemons.FirstOrDefault(p => p.Name == name);
    }

    public double GetPokemonRating(int id) 
    {
        return GetPokemon(id)?.Reviews?.Average(r => r.Rating) ?? 0;
    }

    public bool PokemonExists(int id) 
    {
        return _context.Pokemons.Where(p => p.Id == id).Count() > 0;
    }

    public bool UpdatePokemon(Pokemon pokemon)
    {
        _context.Update(pokemon);

        return Save();
    }

    public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
    {
        // Дві id мають проходити валідацію в контролері
        var pokemonOwnerEntity = _context.Owners.First(o => o.Id == ownerId);
        var pokemonCategoryEntity = _context.Categories.First(c => c.Id == categoryId);
        pokemon.Id = 0;
        
        _context.Add(new PokemonOwner()
        {
            //PokemonId = pokemon.Id,
            Pokemon = pokemon,
            //OwnerId = ownerId,
            Owner = pokemonOwnerEntity
        });
        
        _context.Add(new PokemonCategory()
        {
            //PokemonId = pokemon.Id,
            Pokemon = pokemon,
            //CategoryId = categoryId,
            Category = pokemonCategoryEntity
        });

        _context.Add(pokemon);
        return Save();
    }
    public bool Save()
    {
        var saveChanges = _context.SaveChanges();
        return saveChanges > 0 ? true : false;
    }
}