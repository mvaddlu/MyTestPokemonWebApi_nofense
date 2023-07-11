public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int id);
    Pokemon GetPokemon(string name);
    double GetPokemonRating(int id);
    bool PokemonExists(int id);
    bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
    bool Save();
}