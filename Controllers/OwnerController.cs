using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : Controller
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    public OwnerController(IOwnerRepository ownerRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
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
}