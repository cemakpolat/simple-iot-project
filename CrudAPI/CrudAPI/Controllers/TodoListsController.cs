using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CrudAPI.Domain;
using CrudAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudAPI.Controllers
{
    
    [Route("api/[controller]")]
    [Controller]
    public class TodoListsController : Controller
    {
        
        
        private readonly TodoListService _todoListService;

        
        public TodoListsController(TodoListService todoListService)
        {
            _todoListService = todoListService;
        }
        
        
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody]TodoList todoList)
        {
            var result = await _todoListService.Insert(todoList);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _todoListService.GetAll();
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var result = await _todoListService.GetById(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]TodoList todoList)
        {
            var result = await _todoListService.Update(todoList);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _todoListService.Remove(id);
            return Ok();
        }
        
    }
}