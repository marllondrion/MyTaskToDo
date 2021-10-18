using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTaskToDo.Data;
using MyTaskToDo.Model;
using MyTaskToDo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTaskToDo.Controllers
{
    [ApiController]
    [Route(template:"v1")]
    
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route(template:"todos")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            List<Todo> todos = await context.Todos
                .AsNoTracking()
                .ToListAsync();
            return Ok(todos);
        }

        [HttpGet]
        [Route(template: "todos/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] AppDbContext context, [FromRoute] int id)
        { 
            var todo = await context.Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return todo == null ? NotFound() : Ok(todo);
        }

        [HttpPost]
        [Route(template: "todos")]
        public async Task<IActionResult> PostAsync([FromServices] AppDbContext context, [FromBody] EditeView model)
        {
            if (!ModelState.IsValid) { 
                return BadRequest();
            }


            var todo = new Todo{ 
                Date = DateTime.Now,
                Done = false,
                Title = model.Title };
            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created(uri: $"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest();

            }

        }


        [HttpPut]
        [Route(template: "todos/{id}")]
        public async Task<IActionResult> PutAsync([FromServices] AppDbContext context, [FromBody] EditeView model, [FromRoute] int id)
        {
            if (!ModelState.IsValid || id != model.Id)
            {
                return BadRequest();
            }


            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
                return NotFound();

            try
            {
                todo.Title = model.Title;
                
                context.Todos.Update(todo);
                await context.SaveChangesAsync();
                
                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest();

            }

        }

        [HttpDelete]
        [Route(template:"todos/{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
                return NotFound();
            try { 
            context.Todos.Remove(todo);
            context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }


        }

    }
}
