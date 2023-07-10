public interface IReviewRepository
{
    ICollection<Review> GetReviews();
    ICollection<Review> GetReviewsOnPokemon(int pokemonId);
    Review? GetReview(int reviewId);
    bool ReviewExists(int reviewId);
}