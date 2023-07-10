using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    public ReviewController(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }
    [HttpGet("{reviewId}")]
    [ProducesResponseType(typeof(ReviewDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReview(int reviewId, CancellationToken cancellationToken)
    {
        if(_reviewRepository.ReviewExists(reviewId) == false)
        {
            return NotFound();
        }

        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(review);
    }

    [HttpGet("reviews")]
    [ProducesResponseType(typeof(ICollection<ReviewDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviews(CancellationToken cancellationToken)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviews);
    }

    [HttpGet("pokemon/{pokemonId}")]
    [ProducesResponseType(typeof(ICollection<ReviewDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewsOnPokemon(int pokemonId, CancellationToken cancellationToken)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOnPokemon(pokemonId));

        if(reviews is null)
        {
            return NotFound();
        }

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviews);
    }
}