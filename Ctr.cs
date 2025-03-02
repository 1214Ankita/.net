// Controllers/DishesController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DishApi.Models;
using DishApi.Repositories;

namespace DishApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly ExcelDishRepository _repository;

        public DishesController()
        {
            _repository = new ExcelDishRepository();
        }

        [HttpGet]
        public async Task<ActionResult<List<Dish>>> GetAllDishes()
        {
            var dishes = await _repository.GetAllDishesAsync();
            return Ok(dishes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            var dish = await _repository.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return Ok(dish);
        }

        [HttpPost]
        public async Task<ActionResult> AddDish([FromBody] Dish dish)
        {
            await _repository.AddDishAsync(dish);
            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDish(int id, [FromBody] Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest();
            }
            await _repository.UpdateDishAsync(dish);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDish(int id)
        {
            await _repository.DeleteDishAsync(id);
            return NoContent();
        }
    }
}
