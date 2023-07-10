public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();
    Owner? GetOwner(int ownerId);
    ICollection<Pokemon> GetPokemonsByOwner(int ownerId);
    Owner? GetOwnerOfAPokemon(int pokemonId);
    bool OwnerExists(int ownerId);
}