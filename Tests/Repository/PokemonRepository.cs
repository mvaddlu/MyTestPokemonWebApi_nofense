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
            }
            await dataContext.SaveChangesAsync();
        }
        return dataContext;
    }
    [Fact]
    public async void PokemonRepository_GetPokemon_ReturnPokemon()
    {
        //Arrange
        var name = "Pikachu";

        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);
        //Act
        var result = pokemonRepository.GetPokemon(name);
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
    
}