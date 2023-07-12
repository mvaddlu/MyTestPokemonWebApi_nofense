using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PokemonsController : Controller
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public PokemonsController(IPokemonRepository pokemonRepository, 
        ICategoryRepository categoryRepository, IOwnerRepository ownerRepository, IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _ownerRepository = ownerRepository;
        _categoryRepository = categoryRepository;
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

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult CreatePokemon([FromQuery]int ownerId, [FromQuery]int categoryId, [FromBody]PokemonDto pokemonCreate)
    {
        if(pokemonCreate is null)
        {
            return BadRequest(ModelState);
        }

        if(_categoryRepository.CategoryExists(categoryId) == false)
        {
            ModelState.AddModelError("", $"Category by id:{categoryId} does not exists");
            return StatusCode(422, ModelState);
        }

        if(_ownerRepository.OwnerExists(ownerId) == false)
        {
            ModelState.AddModelError("", $"Owner by id:{ownerId} does not exists");
            return StatusCode(422, ModelState);
        }

        if(_pokemonRepository.CheckIfPokemonExistByName(pokemonCreate.Name))
        {
            ModelState.AddModelError("", $"Pokemon by name:{pokemonCreate.Name} already exists");
            return StatusCode(422, ModelState);
        }

        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

        if(_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{pokemonId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdatePokemon([FromRoute]int pokemonId, [FromBody]PokemonDto pokemonUpdate)
    {
        if(pokemonUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_pokemonRepository.PokemonExists(pokemonId) == false)
        {
            ModelState.AddModelError("", $"Pokemon by id:{pokemonId} does not exists");
            return StatusCode(422, ModelState);
        }
        pokemonUpdate.Id = pokemonId;

        var pokemonMap = _pokemonRepository.GetPokemon(pokemonId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        pokemonMap.Name = pokemonUpdate.Name;
        pokemonMap.BirthDate = pokemonUpdate.BirthDate;

        if(_pokemonRepository.UpdatePokemon(pokemonMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }

    [HttpDelete("{pokemonId}")]
    [ProducesResponseType(284)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner([FromRoute]int pokemonId)
    {
        if(_pokemonRepository.PokemonExists(pokemonId) == false)
        {
            return NotFound();
        }

        var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId)!;

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        if(_pokemonRepository.DeletePokemon(pokemonToDelete) == false)
        {
            ModelState.AddModelError("", "Something went wrong during deletion");
            return StatusCode(500, ModelState);
        }

        return StatusCode(284);
    }
}