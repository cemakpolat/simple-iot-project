using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataStore.Domain;
using DataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace DataStore.Controllers
{
    
    [Route("api/[controller]")]
    [Controller]
    public class EntityController : Controller
    {
        
        
        private readonly IoTService _iotService;

        
        public EntityController(IoTService service)
        {
            _iotService = service;
        }
        
      /*  
      // TODO: Actual Entity and incoming json cannot be properly converted.
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody]Entity entity)
        {
            var result = await _iotService.Insert(et);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    */
    
    
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _iotService.GetAll();
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var result = await _iotService.GetById(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]Entity list)
        {
            var result = await _iotService.Update(list);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _iotService.Remove(id);
            return Ok();
        }
        
    }
}