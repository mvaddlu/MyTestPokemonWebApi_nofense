using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IMapper _mapper;
    public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _reviewerRepository = reviewerRepository;
        _pokemonRepository = pokemonRepository;
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


    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult CreateReview([FromQuery]int reviewerId, [FromQuery]int pokemonId, [FromBody]ReviewDto reviewCreate, CancellationToken cancellationToken)
    {
        if(reviewCreate is null)
        {
            return BadRequest(ModelState);
        }

        // Використовувати лише якщо 
        //if(_OwnerRepository.OwnerExists(ownerCreate.Id))
        //{
        //    ModelState.AddModelError("", $"Owner by id:{ownerCreate.Id} alredy exists");
        //    return StatusCode(422, ModelState);
        //}

        var review = _reviewRepository
            .GetReviews()
            .Where(r => r.Title.Trim()
                .Equals(r.Title.Trim(), StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

        if(review is not null)
        {
            ModelState.AddModelError("", $"Review by title:{review.Title} alredy exists");
            return StatusCode(422, ModelState);
        }

        var reviewMap = _mapper.Map<Review>(reviewCreate);

        if(_reviewerRepository.ReviewerExists(reviewerId) == false)
        {
            ModelState.AddModelError("", $"Reviewer by id:{reviewerId} does not exists");
            return StatusCode(422, ModelState);
        }

        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId)!;

        if(_pokemonRepository.PokemonExists(pokemonId) == false)
        {
            ModelState.AddModelError("", $"Pokemon by id:{pokemonId} does not exists");
            return StatusCode(422, ModelState);
        }

        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokemonId)!;

        if(_reviewRepository.CreateReview(reviewMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdateReview([FromRoute]int reviewId, [FromBody]ReviewDto reviewUpdate)
    {
        if(reviewUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_reviewRepository.ReviewExists(reviewId) == false)
        {
            ModelState.AddModelError("", $"Review by id:{reviewId} does not exists");
            return StatusCode(422, ModelState);
        }
        reviewUpdate.Id = reviewId;

        var reviewMap = _reviewRepository.GetReview(reviewId)!;
        reviewMap.Rating = reviewUpdate.Rating;
        reviewMap.Text = reviewUpdate.Text;
        reviewMap.Title = reviewUpdate.Title;

        if(_reviewRepository.UpdateReview(reviewMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }
}