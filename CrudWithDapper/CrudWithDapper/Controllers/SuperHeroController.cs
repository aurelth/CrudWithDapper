using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CrudWithDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        private ILogger<SuperHeroController> _logger;
        public SuperHeroController(IConfiguration config, ILogger<SuperHeroController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuperHero>>> GetSuperHeroes()
        {
            _logger.LogInformation("Getting all Super Heroes from Database");

            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var heroes = await SelectAllHeroes(connection);
                _logger.LogInformation("Super Heroes retrieved with success");
                return Ok(heroes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while getting the Super Heroes from the Database.");
                throw new Exception(ex.Message);
            }            
        }

        [HttpGet]
        [Route("{heroId:int}")]
        public async Task<ActionResult<SuperHero>> GetSuperHero([FromRoute] int heroId)
        {
            _logger.LogInformation($"Getting the Super Hero with the Id {heroId} from the Database");

            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var hero = await connection.QueryFirstAsync<SuperHero>("select * from SuperHeroes where id = @Id", new { Id = heroId });
                _logger.LogInformation("Super Hero retrieved with success");
                return Ok(hero);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting the Super Hero with the Id {heroId} from the Database.");
                throw new Exception(ex.Message);
            }            
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> CreateHero([FromBody] SuperHero hero)
        {
            _logger.LogInformation($"Inserting a new Super Hero into the Database");

            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var newHero = await connection.ExecuteAsync("insert into SuperHeroes (Name, FirstName, LastName, Place) values (@Name, @FirstName, @LastName, @Place)", hero);
                _logger.LogInformation("Super Hero created with success");
                return Ok(await SelectAllHeroes(connection));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while inserting the new Super Hero into the Database.");
                throw new Exception(ex.Message);
            }            
        }

        [HttpPut]        
        public async Task<ActionResult<SuperHero>> UpdateHero([FromBody] SuperHero hero)
        {
            _logger.LogInformation($"Updating the Super Hero with the Id {hero.Id} in the Database");

            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var newHero = await connection.ExecuteAsync("update SuperHeroes set Name = @Name, FirstName = @FirstName, LastName = @LastName, Place = @Place where id = Id", hero);
                _logger.LogInformation("Super Hero updated with success");
                return Ok(await SelectAllHeroes(connection));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the Super Hero with the Id {hero.Id} in the Database.");
                throw new Exception(ex.Message);
            }            
        }

        [HttpDelete]
        [Route("{heroId:int}")]
        public async Task<ActionResult<SuperHero>> DeleteHero([FromRoute] int heroId)
        {
            _logger.LogInformation($"Deleting the Super Hero with the Id {heroId} in the Database");

            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var newHero = await connection.ExecuteAsync("delete from SuperHeroes where id = @Id", new { Id = heroId });
                _logger.LogInformation("Super Hero deleted with success");
                return Ok(await SelectAllHeroes(connection));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the Super Hero with the Id {heroId} in the Database.");
                throw new Exception(ex.Message);
            }            
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection sqlConnection)
            => await sqlConnection.QueryAsync<SuperHero>("select * from SuperHeroes");
    }
}
