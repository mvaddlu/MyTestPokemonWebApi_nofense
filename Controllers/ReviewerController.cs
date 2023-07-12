using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PokemonApi.Controllers;

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


    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult CreateReviewer([FromBody]ReviewerDto reviewerCreate, CancellationToken cancellationToken)
    {
        if(reviewerCreate is null)
        {
            return BadRequest(ModelState);
        }

        // Використовувати лише якщо 
        //if(_OwnerRepository.OwnerExists(ownerCreate.Id))
        //{
        //    ModelState.AddModelError("", $"Owner by id:{ownerCreate.Id} alredy exists");
        //    return StatusCode(422, ModelState);
        //}

        var reviewer = _reviewerRepository
            .GetReviewers()
            .Where(r => $"{r.FirstName} {r.LastName}".Trim()
                .Equals($"{reviewerCreate.FirstName} {reviewerCreate.LastName}".Trim(), StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

        if(reviewer is not null)
        {
            ModelState.AddModelError("", $"Reviewer by name:{reviewer.FirstName} {reviewer.LastName} alredy exists");
            return StatusCode(422, ModelState);
        }

        var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

        if(_reviewerRepository.CreateReviewer(reviewerMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{reviewerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdateReviewer([FromRoute]int reviewerId, [FromBody]ReviewerDto reviewerUpdate)
    {
        if(reviewerUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_reviewerRepository.ReviewerExists(reviewerId) == false)
        {
            ModelState.AddModelError("", $"Reviewer by id:{reviewerId} does not exists");
            return StatusCode(422, ModelState);
        }
        reviewerUpdate.Id = reviewerId;

        var reviewerMap = _reviewerRepository.GetReviewer(reviewerId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        reviewerMap.FirstName = reviewerUpdate.FirstName;
        reviewerMap.LastName = reviewerUpdate.LastName;

        if(_reviewerRepository.UpdateReviewer(reviewerMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }

    [HttpDelete("{reviewerId}")]
    [ProducesResponseType(284)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner([FromRoute]int reviewerId)
    {
        if(_reviewerRepository.ReviewerExists(reviewerId) == false)
        {
            return NotFound();
        }

        var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        if(_reviewerRepository.DeleteReviewer(reviewerToDelete) == false)
        {
            ModelState.AddModelError("", "Something went wrong during deletion");
            return StatusCode(500, ModelState);
        }

        return StatusCode(284);
    }
}