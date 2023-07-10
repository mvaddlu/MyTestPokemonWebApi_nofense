using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : Controller
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;
    public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
    {
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }
    [HttpGet("{reviewerId}")]
    [ProducesResponseType(typeof(ReviewerDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewer(int reviewerId, CancellationToken cancellationToken)
    {
        if(_reviewerRepository.ReviewerExists(reviewerId) == false)
        {
            return NotFound();
        }

        var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviewer);
    }

    [HttpGet("reviewers")]
    [ProducesResponseType(typeof(ICollection<ReviewerDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewers(CancellationToken cancellationToken)
    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviewers);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(typeof(ICollection<ReviewDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetReviewsByReviewer(int reviewerId, CancellationToken cancellationToken)
    {
        if(_reviewerRepository.ReviewerExists(reviewerId) == false)
        {
            return NotFound();
        }

        var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviews);
    }
}