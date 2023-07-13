using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace PokemonApi.Tests.Controllers;

public class PokemonsControllerTests
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly CancellationToken _cancellationToken;
    private readonly PokemonsController _pokemonController;
    public PokemonsControllerTests()
    {
        _pokemonRepository = A.Fake<IPokemonRepository>();
        _ownerRepository = A.Fake<IOwnerRepository>();
        _categoryRepository = A.Fake<ICategoryRepository>();
        _mapper = A.Fake<IMapper>();

        _pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
    }

    [Fact]
    public void PokemonController_GetPokemons_ReturnOk()
    {
        #region Arrange

        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonsList = A.Fake<List<PokemonDto>>();
        A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonsList);
        var controller = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
        
        #endregion
        #region Act

        var result = controller.GetPokemons(new CancellationToken());
        
        #endregion
        #region Assert
        
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(OkObjectResult));
        #endregion
    }
    [Fact]
    public void PokemonController_CreatePokemon_ReturnOk()
    {
        #region Arrange

        int ownerId = 1;
        int categoryId = 2;
        var pokemon = A.Fake<Pokemon>();
        var pokemonCreate = A.Fake<PokemonDto>();

        var controller = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
        
        ICollection<Pokemon> pokemons = A.Fake<ICollection<Pokemon>>();
        A.CallTo(() => _pokemonRepository.GetPokemons()).Returns(pokemons);

        A.CallTo(() => _ownerRepository.OwnerExists(ownerId)).Returns(true);
        A.CallTo(() => _categoryRepository.CategoryExists(categoryId)).Returns(true);
        A.CallTo(() => _pokemonRepository.CheckIfPokemonExistByName(pokemonCreate.Name)).Returns(false);
        
        A.CallTo(() => _pokemonRepository.GetPokemons());
        
        A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemon);
        
        A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemon)).Returns(true);
        
        #endregion
        #region Act
        var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);
        #endregion
        #region Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        #endregion
    }
    [Fact]
    public void PokemonController_GetPokemonRating_ReturnOk()
    {
        // Arrange
        int pokemonId = 1;
        var pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
        double rating = 1;

        A.CallTo(() => _pokemonRepository.PokemonExists(pokemonId)).Returns(true);
        A.CallTo(() => _pokemonRepository.GetPokemonRating(pokemonId)).Returns(rating);
        // Act
        var result = pokemonController.GetPokemonRating(pokemonId, new CancellationToken());
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var resultObject = (OkObjectResult)result;
        resultObject.Value.Should().BeOfType<double>();
        ((double)(resultObject.Value!)).Should().Be(rating);
    }
    [Fact]
    public void PokemonController_UpdatePokemon_ReturnOk()
    {
        // Arrange
        int pokemonId = 1;
        var pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);

        var pokemonUpdate = new PokemonDto()
        {
            Name = Guid.NewGuid().ToString()
        };
        var pokemon = new Pokemon()
        {
            Name = Guid.NewGuid().ToString()
        };

        A.CallTo(() => _pokemonRepository.PokemonExists(pokemonId)).Returns(true);
        A.CallTo(() => _pokemonRepository.GetPokemon(pokemonId)).Returns(pokemon);
        A.CallTo(() => _pokemonRepository.UpdatePokemon(pokemon)).Returns(true);
        // Act
        var result = pokemonController.UpdatePokemon(pokemonId, pokemonUpdate);
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    [Fact]
    public void PokemonController_DeletePokemon_ReturnObjectResult()
    {
        //Arrange
        int pokemonId = 1;
        Pokemon pokemon = new Pokemon();
        var pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);

        A.CallTo(() => _pokemonRepository.PokemonExists(pokemonId)).Returns(true);
        A.CallTo(() => _pokemonRepository.GetPokemon(pokemonId)).Returns(pokemon);
        A.CallTo(() => _pokemonRepository.DeletePokemon(pokemon)).Returns(true);
        // Act
        var result = pokemonController.DeletePokemon(pokemonId);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.Should().Be(284);
    }
    [Fact]
    public void PokemonController_GetPokemonByName_ReturnOk()
    {
        // Arrange
        string pokemonName = "Pikachu";
        PokemonDto pokemon  = new PokemonDto();
        var pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
        A.CallTo(() => _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokemonName))).Returns(pokemon);
        // Act
        var result = pokemonController.GetPokemonByName(pokemonName, new CancellationToken());
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void PokemonController_GetPokemon_ReturnOk()
    {
        // Arrange
        int pokemonId = 1;
        PokemonDto pokemon  = new PokemonDto();
        var pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
        A.CallTo(() => _pokemonRepository.PokemonExists(pokemonId)).Returns(true);
        A.CallTo(() => _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokemonId))).Returns(pokemon);
        // Act
        var result = pokemonController.GetPokemon(pokemonId, new CancellationToken());
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
    }
}