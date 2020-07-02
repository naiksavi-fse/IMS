using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IDatabase _database;

        public CacheController(IDatabase database)
        {
            _database = database;
        }

        // GET: api/Cache?key=key
        [HttpGet]
        public string Get([FromQuery] string key)
        {
            return _database.StringGet(key);
        }

        // POST: api/Cache
        [HttpPost]
        public void Post([FromBody] KeyValuePair<string, string> keyValue)
        {
            _database.StringSet(keyValue.Key, keyValue.Value);
        }
    }
}
