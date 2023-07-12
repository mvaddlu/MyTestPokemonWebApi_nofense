namespace PokemonApi.Repositories;

public class CategoryRepository : ICategoryRepository 
{
    private readonly DataContext _context;
    public CategoryRepository(DataContext context)
    {
        _context = context;
    }
    public bool CategoryExists(int id) 
    {
        return _context.Categories.Any(c => c.Id == id);
    }

    public ICollection<Category> GetCategories() 
    {
        return _context.Categories.OrderBy(c => c.Id).ToList();
    }

    public Category GetCategory(int id) 
    {
        return _context.Categories.FirstOrDefault(c => c.Id == id);
    }

    public Category GetCategory(string name)
    {
        return _context.Categories.FirstOrDefault(c => c.Name == name);
    }

    public ICollection<Pokemon> GetPokemons(int categoryId) 
    {
        var category = GetCategory(categoryId);

        return _context.PokemonCategories
            .Where(pc => pc.CategoryId == categoryId)
            .Select(pc => pc.Pokemon)
            .ToList();
    }
    public bool CreateCategory(Category category)
    {
        category.Id = 0;
        _context.Add(category);
        return Save();
    }
    public bool UpdateCategory(Category category)
    {
        _context.Update(category);

        return Save();
    }
    public bool DeleteCategory(Category category)
    {
        _context.Remove(category);

        return Save();
    }
    public bool Save()
    {
        var saveChanges = _context.SaveChanges();
        return saveChanges > 0 ? true : false; 
    }
}