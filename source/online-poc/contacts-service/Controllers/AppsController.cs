using ContactsService.Data;
using ContactsService.Extensions;
using ContactsService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ContactsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppsController : ControllerBase
    {
        private readonly IMongoDBContext _mongoDBContext;
        private readonly IRepository<State> _repository;

        public AppsController(IMongoDBContext mongoDBContext, IRepository<State> repository)
        {
            _mongoDBContext = mongoDBContext;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AppStarted([FromBody] AppStarted appEvent)
        {
            //var result = _mongoDBContext.Database().GetCollection<Contact>(typeof(Contact).Name).AsQueryable();
            State? result = await _repository.UpdateAsync(new State { Id = appEvent?.UniqueId, LastLoggedIn = DateTime.Now.ToUnixTimestamp() });

            return Ok(result);
        }
    }
}