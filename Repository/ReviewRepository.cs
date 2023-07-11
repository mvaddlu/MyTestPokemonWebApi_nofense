public class ReviewRepository : IReviewRepository {
    private readonly DataContext _context;
    public ReviewRepository(DataContext context)
    {
        _context = context;
    }

    public Review? GetReview(int reviewId) 
    {
        return _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
    }

    public ICollection<Review> GetReviews() 
    {
        return _context.Reviews.ToList();
    }

    public ICollection<Review> GetReviewsOnPokemon(int pokemonId) 
    {
        return _context.Reviews.Where(r => r.Pokemon.Id == pokemonId).ToList();
    }

    public bool ReviewExists(int reviewId) 
    {
        return _context.Reviews.Any(r => r.Id == reviewId);
    }

    public bool CreateReview(Review review)
    {
        _context.Add(review);
        return Save();
    }

    public bool UpdateReview(Review review)
    {
        _context.Update(review);
        return Save();
    }

    public bool Save()
    {
        var saveChanges = _context.SaveChanges();
        return saveChanges > 0 ? true : false;
    }
}