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
        public IEnumerable<Greeting> Get()
        {
            return _greetingRepository.Get();
           
        }

        // GET api/<GreetingController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            var greeting = _greetingRepository.Get(id);
            if (greeting == null)
                return NotFound();

            return Ok(greeting);
        }

        // POST api/<GreetingController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Post ([FromBody] Greeting greeting)
        {
            try
            {
                _greetingRepository.Create(greeting);
                return Created("http://localhost:5002/api/Greeting", greeting);
            }
            catch (Exception)
            {
                return Conflict();
            }
        }

        // PUT api/<GreetingController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted,Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put([FromBody] Greeting greeting)
        {
            try
            {
                _greetingRepository.Update(greeting);
                return Accepted(greeting);
            }
            catch (Exception)
            {
                return NotFound($"Greeting with id {greeting.Id} not found!");
            }
        }

        // DELETE api/<GreetingController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            try
            {
            _greetingRepository.Delete(id);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound($"Greeting with id {id} was not found!");
            }
        }

        // DELETE api/<GreetingController>/5
        [HttpDelete]
        public void DeleteAll()
        {
            _greetingRepository.DeleteAll();
        }
    }
}
