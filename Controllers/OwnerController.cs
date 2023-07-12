using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }
    [HttpGet("owners")]
    [ProducesResponseType(typeof(ICollection<OwnerDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetOwners(CancellationToken cancellationToken)
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owners);
    }


    [HttpGet("{ownerId}")]
    [ProducesResponseType(typeof(OwnerDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int ownerId, CancellationToken cancellationToken)
    {
        if(_ownerRepository.OwnerExists(ownerId) == false)
        {
            return NotFound();
        }

        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owner);
    }


    [HttpGet("pokemon/{pokemonId}/owner")]
    [ProducesResponseType(typeof(OwnerDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetOwnerByPokemon(int pokemonId, CancellationToken cancellationToken)
    {
        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwnerOfAPokemon(pokemonId));

        if(owner is null)
        {
            return NotFound();
        }

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owner);
    }
    
    [HttpGet("{ownerId}/pokemons")]
    [ProducesResponseType(typeof(ICollection<PokemonDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonsByOwner(int ownerId, CancellationToken cancellationToken)
    {
        if(_ownerRepository.OwnerExists(ownerId) == false)
        {
            return NotFound();
        }

        var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonsByOwner(ownerId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemons);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult CreateOwner([FromQuery]int countryId, [FromBody]OwnerDto ownerCreate, CancellationToken cancellationToken)
    {
        if(ownerCreate is null)
        {
            return BadRequest(ModelState);
        }

        // Використовувати лише якщо 
        //if(_OwnerRepository.OwnerExists(ownerCreate.Id))
        //{
        //    ModelState.AddModelError("", $"Owner by id:{ownerCreate.Id} alredy exists");
        //    return StatusCode(422, ModelState);
        //}

        var owner = _ownerRepository
            .GetOwners()
            .Where(o => $"{o.FirstName} {o.LastName}".Trim()
                .Equals($"{ownerCreate.FirstName} {ownerCreate.LastName}".Trim(), StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

        if(owner is not null)
        {
            ModelState.AddModelError("", $"Owner by name:{ownerCreate.FirstName} {ownerCreate.LastName} alredy exists");
            return StatusCode(422, ModelState);
        }

        var ownerMap = _mapper.Map<Owner>(ownerCreate);

        if(_countryRepository.CountryExists(countryId) == false)
        {
            ModelState.AddModelError("", $"Country by id:{countryId} does not exists");
            return StatusCode(422, ModelState);
        }

        ownerMap.Country = _countryRepository.GetCountry(countryId)!;

        if(_ownerRepository.CreateOwner(ownerMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }
    
    [HttpPut("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdateCountry([FromRoute]int ownerId, [FromBody]OwnerDto ownerUpdate)
    {
        if(ownerUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_ownerRepository.OwnerExists(ownerId) == false)
        {
            ModelState.AddModelError("", $"Owner by id:{ownerId} does not exists");
            return StatusCode(422, ModelState);
        }
        ownerUpdate.Id = ownerId;

        if(_ownerRepository
            .GetOwners()
            .Any(o => $"{o.FirstName.Trim()} {o.LastName.Trim()} {o.Gym.Trim()}"
            .Equals($"{ownerUpdate.FirstName.Trim()} {ownerUpdate.LastName.Trim()} {ownerUpdate.Gym.Trim()}", StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("", $"Owner by params:{ownerUpdate.FirstName.Trim()} {ownerUpdate.LastName.Trim()} {ownerUpdate.Gym.Trim()} already exists");
            return StatusCode(422, ModelState);
        }

        var ownerMap = _ownerRepository.GetOwner(ownerId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }
        
        ownerMap.FirstName = ownerUpdate.FirstName;
        ownerMap.LastName = ownerUpdate.LastName;
        ownerMap.Gym = ownerUpdate.Gym;

        if(_ownerRepository.UpdateOwner(ownerMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }
    [HttpDelete("{ownerId}")]
    [ProducesResponseType(284)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner([FromRoute]int ownerId)
    {
        if(_ownerRepository.OwnerExists(ownerId) == false)
        {
            return NotFound();
        }

        var ownerToDelete = _ownerRepository.GetOwner(ownerId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        if(_ownerRepository.DeleteOwner(ownerToDelete) == false)
        {
            ModelState.AddModelError("", "Something went wrong during deletion");
            return StatusCode(500, ModelState);
        }

        return StatusCode(284);
    }
}