using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CountryController : Controller
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    public CountryController(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }
    [HttpGet("countries")]
    [ProducesResponseType(typeof(ICollection<CountryDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetCountries(CancellationToken cancellationToken)
    {
        var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(countries);
    }

    [HttpGet("{countryId}")]
    [ProducesResponseType(typeof(CountryDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetCountry(int countryId, CancellationToken cancellationToken)
    {
        if(_countryRepository.CountryExists(countryId) == false)
        {
            return NotFound();
        }

        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(country);
    }

    [HttpGet("{countryId}/owners")]
    [ProducesResponseType(typeof(ICollection<OwnerDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetOwners(int countryId, CancellationToken cancellationToken)
    {
        if(_countryRepository.CountryExists(countryId) == false)
        {
            return NotFound();
        }

        var owners = _mapper.Map<List<OwnerDto>>(_countryRepository.GetOwners(countryId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owners);
    }

    [HttpGet("byOwner/{ownerId}")]
    [ProducesResponseType(typeof(ICollection<CountryDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetCountryByOwner(int ownerId, CancellationToken cancellationToken)
    {
        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

        if(country is null)
        {
            return NotFound();
        }

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(country);
    }
}