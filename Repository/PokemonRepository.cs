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
}