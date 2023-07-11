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


    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult CreateCountry([FromBody]CountryDto countryCreate)
    {
        if(countryCreate is null)
        {
            return BadRequest(ModelState);
        }

        // Використовувати лише якщо 
        //if(_countryRepository.CountryExists(countryCreate.Id))
        //{
        //    ModelState.AddModelError("", $"Country by id:{categoryCreate.Id} alredy exists");
        //    return StatusCode(422, ModelState);
        //}

        var country = _countryRepository
            .GetCountries()
            .Where(c => c.Name.Trim()
                .Equals(countryCreate.Name.Trim(), StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

        if(country is not null)
        {
            ModelState.AddModelError("", $"Country by name:{countryCreate.Name} alredy exists");
            return StatusCode(422, ModelState);
        }

        var countryMap = _mapper.Map<Country>(countryCreate);

        if(_countryRepository.CreateCountry(countryMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{countryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdateCountry([FromRoute]int countryId, [FromBody]CountryDto countryUpdate)
    {
        if(countryUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_countryRepository.CountryExists(countryId) == false)
        {
            ModelState.AddModelError("", $"Country by id:{countryId} does not exists");
            return StatusCode(422, ModelState);
        }
        countryUpdate.Id = countryId;

        if(_countryRepository.GetCountries().Any(c => c.Name.Trim().Equals(countryUpdate.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("", $"Country by name:{countryUpdate.Name} already exists");
            return StatusCode(422, ModelState);
        }

        var countryMap = _countryRepository.GetCountry(countryId)!;
        countryMap.Name = countryUpdate.Name;

        if(_countryRepository.UpdateCountry(countryMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }
}