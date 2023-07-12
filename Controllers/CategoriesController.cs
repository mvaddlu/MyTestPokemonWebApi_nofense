using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    [HttpGet("{categoryId}/category")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetCategory(int categoryId, CancellationToken cancellationToken)
    {
        if(_categoryRepository.CategoryExists(categoryId) == false)
        {
            return NotFound(new { ErrorMessage = $"Category by id:{categoryId} does not exist" });
        }

        var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(category);
    }

        [HttpGet("categories")]
    [ProducesResponseType(typeof(ICollection<CategoryDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetCategories(CancellationToken cancellationToken)
    {
        var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(categories);
    }

    [HttpGet("{categoryName}/categoryByName")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetCategory(string categoryName, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryName));

        if(category is null)
        {
            return NotFound(new { ErrorMessage = $"Category by name:{categoryName} does not exist" });
        }


        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(category);
    }
    

    [HttpGet("{categoryId}/category/pokemons")]
    [ProducesResponseType(typeof(ICollection<PokemonDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetPokemons(int categoryId, CancellationToken cancellationToken)
    {
        if(_categoryRepository.CategoryExists(categoryId) == false)
        {
            return NotFound(new { ErrorMessage = $"Category by id:{categoryId} does not exist" });
        }

        var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemons(categoryId));

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
    public IActionResult CreateCategory([FromBody]CategoryDto categoryCreate)
    {
        if(categoryCreate is null)
        {
            return BadRequest(ModelState);
        }

        if(_categoryRepository.CategoryExists(categoryCreate.Id))
        {
            ModelState.AddModelError("", $"Category by id:{categoryCreate.Id} alredy exists");
            return StatusCode(422, ModelState);
        }

        var category = _categoryRepository
            .GetCategories()
            .Where(c => c.Name.Trim()
                .Equals(categoryCreate.Name.Trim(), StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

        if(category is not null)
        {
            ModelState.AddModelError("", $"Category by name:{categoryCreate.Name} alredy exists");
            return StatusCode(422, ModelState);
        }

        var categoryMap = _mapper.Map<Category>(categoryCreate);

        if(_categoryRepository.CreateCategory(categoryMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during saving process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created");
    }

    [HttpPut("{categoryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public IActionResult UpdateCategory([FromRoute]int categoryId, [FromBody]CategoryDto categoryUpdate)
    {
        if(categoryUpdate is null)
        {
            return BadRequest(ModelState);
        }

        if(_categoryRepository.CategoryExists(categoryId) == false)
        {
            ModelState.AddModelError("", $"Category by id:{categoryId} does not exists");
            return StatusCode(422, ModelState);
        }
        categoryUpdate.Id = categoryId;

        if(_categoryRepository.GetCategories().Any(c => c.Name.Trim().Equals(categoryUpdate.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("", $"Category by name:{categoryUpdate.Name} already exists");
            return StatusCode(422, ModelState);
        }

        var categoryMap = _categoryRepository.GetCategory(categoryId)!;

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        categoryMap.Name = categoryUpdate.Name;

        if(_categoryRepository.UpdateCategory(categoryMap) == false)
        {
            ModelState.AddModelError("", "Something went wrong during updating process");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Updated");
    }
    [HttpDelete("{categoryId}")]
    [ProducesResponseType(284)]
    [ProducesResponseType(404)]
    public IActionResult DeleteCategory([FromRoute]int categoryId)
    {
        if(_categoryRepository.CategoryExists(categoryId) == false)
        {
            return NotFound();
        }

        var categoryToDelete = _categoryRepository.GetCategory(categoryId);

        if(ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        if(_categoryRepository.DeleteCategory(categoryToDelete) == false)
        {
            ModelState.AddModelError("", "Something went wrong during deletion");
            return StatusCode(500, ModelState);
        }

        return StatusCode(284);
    }
}