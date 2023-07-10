public interface ICategoryRepository
{
    ICollection<Category> GetCategories();
    Category GetCategory(int id);
    Category GetCategory(string name);
    ICollection<Pokemon> GetPokemons(int categoryId);
    bool CategoryExists(int id);
    bool CreateCategory(Category category);
    bool Save();
}