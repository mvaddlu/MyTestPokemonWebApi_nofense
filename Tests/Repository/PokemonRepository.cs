using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace PokemonApi.Tests.Repository;

public class PokemonRepositoryTests
{
    private async Task<DataContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var dataContext = new DataContext(options);
        if(await dataContext.Pokemons.CountAsync() <= 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dataContext.Add(new Pokemon()
                {
                    Name = "Pikachu",
                    BirthDate = new DateTime(1903,1,1),
                    PokemonCategories = new List<PokemonCategory>()
                    {
                        new PokemonCategory { Category = new Category() { Name = "Electric"}}
                    },
                    Reviews = new List<Review>()
                    {
                        new Review { Title="Pikachu",Text = "Pickahu is the best pokemon, because it is electric", Rating = 5,
                        Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith" } },
                        new Review { Title="Pikachu", Text = "Pickachu is the best a killing rocks", Rating = 5,
                        Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones" } },
                        new Review { Title="Pikachu",Text = "Pickchu, pickachu, pikachu", Rating = 1,
                        Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor" } },
                    }
                });
                dataContext.Add(new Owner
                {
                    FirstName = $"fn{i}",
                    LastName = $"ln{i}",
                    Gym = $"g{i}",
                    Country = new Country()
                    {
                        Name = $"c{i}"
                    }
                });
                /*dataContext.Add(new Category()
                {
                    Name = $"c{i}"
                });*/
            }
            await dataContext.SaveChangesAsync();
        }
        return dataContext;
    }
    [Fact]
    public async void PokemonRepository_GetPokemon_ById_ReturnPokemon()
    {
        //Arrange
        var id = 10;

        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        //Act
        var result = pokemonRepository.GetPokemon(id);
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Pokemon>();
    }
    [Fact]
    public async void PokemonRepository_GetPokemonRating_ReturnDouble()
    {
        //Arrange
        var id = 1;

        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);

        //Act
        var result = pokemonRepository.GetPokemonRating(id);
    
        //Assert
        result.Should().NotBe(0);
        result.Should().BeInRange(0, 6);
    }
    [Fact]
    public async void PokemonRepository_GetPokemons_ReturnICollection()
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        //Act
        var result = pokemonRepository.GetPokemons();
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Pokemon>?>();
        result.Should().HaveCountGreaterThan(0);
    }
    [Theory]
    [InlineData("Pikachu")]
    public async void PokemonRepository_GetPokemon_ByString_ReturnPokemon(string pokemonName)
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        //Act
        var result = pokemonRepository.GetPokemon(pokemonName);
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Pokemon>();
        result.Name.Should().Be(pokemonName);
    }
    [Theory]
    [InlineData("Pikachu", true)]
    [InlineData("Pika-pika", false)]
    public async void PokemonRepository_CheckIfPokemonExistByName_ReturnBool(string pokemonName, bool expectedResult)
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        
        //Act
        var result = pokemonRepository.CheckIfPokemonExistByName(pokemonName);
        
        //Assert
        result.Should().Be(expectedResult);
    }
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(22, false)]
    public async void PokemonRepository_UpdatePokemon_ReturnBool(int pokemonId, bool expectedResult)
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        var pokemonToChange = pokemonRepository.GetPokemon(pokemonId);
        //Act
        bool result;
        if(pokemonToChange is null)
            result = false;
        else 
            result = pokemonRepository.UpdatePokemon(pokemonToChange);
        //Assert
        result.Should().Be(expectedResult);
    }
    [Theory]
    [InlineData(10, true)]
    [InlineData(1001, false)]
    public async void PokemonRepository_PokemonExists_ReturnBool(int pokemonId, bool expectedResult)
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        
        //Act
        var result = pokemonRepository.PokemonExists(pokemonId);
        //Assert
        result.Should().Be(expectedResult);
    }
    // Чому воно додає категорії через 2, а не через 1(id)? Біс його знає
    [Theory]
    [InlineData("pika2_2", 2, 2, true)]
    [InlineData("pika4_4", 2, 2, true)]
    [InlineData("pika101_1", 101, 1, false)]
    [InlineData("pika2_101", 2, 101, false)]
    [InlineData("pika101_101", 101, 101, false)]
    public async void PokemonRepository_CreatePokemon_ReturnBool(string pokemonName, 
        int ownerId, int categoryId, bool expectedResult)
    {
        // Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var ownerRepository = new OwnerRepository(dbContext);

        var categories = categoryRepository.GetCategories();
        var categoryExists = categoryRepository.CategoryExists(categoryId);
        var ownerExists = ownerRepository.OwnerExists(ownerId);
        var createPokemon = new Pokemon()
        {
            Name = pokemonName,
            BirthDate = DateTime.Now
        };

        // Act

        var result = categoryExists && ownerExists;
        if(result == true)
        {
            result = pokemonRepository.CreatePokemon(ownerId, categoryId, createPokemon);
        }
    
        // Assert
        result.Should().Be(expectedResult);
    }
    [Theory]
    [InlineData(9, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    [InlineData(12, false)]
    public async void PokemonRepository_DeletePokemon_ReturnBool(int pokemonId, bool expectedResult)
    {
        //Arrange
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);

        //Act
        var result = pokemonRepository.PokemonExists(pokemonId);
        if(result == true)
        {
            var pokemonToDelete = pokemonRepository.GetPokemon(pokemonId);
            result = pokemonRepository.DeletePokemon(pokemonToDelete!);
        }

        //Assert
        result.Should().Be(expectedResult);
    }
}