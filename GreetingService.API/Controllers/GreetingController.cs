using GreetingService.API.Authentication;
using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreetingService.API.Controllers
{
    [Route("api/[controller]")]
    [BasicAuth]
    [ApiController]
    public class GreetingController : ControllerBase
    {
        private readonly IGreetingRepository _greetingRepository;

        public GreetingController(IGreetingRepository greetingRepository)
        {
            _greetingRepository = greetingRepository;
        }

        // GET: api/<GreetingController>
        [HttpGet]
        public async Task <IEnumerable<Greeting>> Get()
        {
            return await _greetingRepository.GetAsync();
           
        }

        // GET api/<GreetingController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> Get(Guid id)
        {
            var greeting = await _greetingRepository.GetAsync(id);
            if (greeting == null)
                return NotFound();

            return Ok(greeting);
        }

        // POST api/<GreetingController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task <IActionResult> Post ([FromBody] Greeting greeting)
        {
            try
            {
                await _greetingRepository.CreateAsync(greeting);
                return Created("http://localhost:5002/api/Greeting", greeting);
            }
            catch (Exception)
            {
                return Conflict();
            }
        }

        // PUT api/<GreetingController>/5
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted,Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> Put([FromBody] Greeting greeting)
        {
            try
            {
                await _greetingRepository.UpdateAsync(greeting);
                return Accepted(greeting);
            }
            catch (Exception)
            {
                return NotFound($"Greeting with id {greeting.id} not found!");
            }
        }

        // DELETE api/<GreetingController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> DeleteAsync(Guid id)
        {
            try
            {
            await _greetingRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound($"Greeting with id {id} was not found!");
            }
        }

        // DELETE api/<GreetingController>/5
        [HttpDelete]
        public async Task DeleteAllAsync()
        {
           await _greetingRepository.DeleteAllAsync();
        }
    }
}
