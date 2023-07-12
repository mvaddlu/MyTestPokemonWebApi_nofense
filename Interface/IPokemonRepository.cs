namespace PokemonApi.Repositories;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int id);
    Pokemon GetPokemon(string name);
    double GetPokemonRating(int id);
    bool PokemonExists(int id);
    bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
    bool CheckIfPokemonExistByName(string name);
    bool UpdatePokemon(Pokemon pokemon);
    bool DeletePokemon(Pokemon pokemon);
    bool Save();
}