public class ReviewerRepository : IReviewerRepository 
{
    private readonly DataContext _context;
    public ReviewerRepository(DataContext context)
    {
        _context = context;
    }
    public Reviewer? GetReviewer(int reviewerId) 
    {
        return _context.Reviewers.FirstOrDefault(r => r.Id == reviewerId);
    }

    public ICollection<Reviewer> GetReviewers() 
    {
        return _context.Reviewers.ToList();
    }

    public ICollection<Review> GetReviewsByReviewer(int reviewerId) 
    {
        return _context.Reviews
            .Where(r => r.Reviewer.Id == reviewerId)
            .ToList();
    }

    public bool ReviewerExists(int reviewerId) 
    {
        return _context.Reviewers.Any(r => r.Id == reviewerId);
    }
}