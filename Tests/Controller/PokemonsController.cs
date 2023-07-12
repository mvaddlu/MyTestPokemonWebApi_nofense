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

        //_pokemonController = new PokemonsController(_pokemonRepository, _categoryRepository, _ownerRepository, _mapper);
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
}