using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PokemonsController : Controller
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IMapper _mapper;
    public PokemonsController(IPokemonRepository pokemonRepository, IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _mapper = mapper;
    }
    [HttpGet("pokemons")]
    [ProducesResponseType(typeof(ICollection<PokemonDto>), 200)]
    [ProducesResponseType(400)]
    public IActionResult GetPokemons(CancellationToken cancellationToken)
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemons);
    }
    [HttpGet("{id}/pokemon")]
    [ProducesResponseType(typeof(PokemonDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int id, CancellationToken cancellationToken)
    {
        if(_pokemonRepository.PokemonExists(id) == false)
            return NotFound();

        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id));

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemon);
    }
    [HttpGet("{name}/pokemon")]
    [ProducesResponseType(typeof(PokemonDto), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetPokemonByName(string name, CancellationToken cancellationToken)
    {
        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(name));

        if(pokemon is null)
        {
            return NotFound();
        }

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemon);
    }
    [HttpGet("{id}/rating")]
    [ProducesResponseType(typeof(double), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int id, CancellationToken cancellationToken)
    {
        if(_pokemonRepository.PokemonExists(id) == false)
            return NotFound();

        double rating = _pokemonRepository.GetPokemonRating(id);

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        return Ok(rating);
    }
}