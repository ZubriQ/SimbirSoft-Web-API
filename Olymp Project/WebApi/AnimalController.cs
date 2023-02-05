using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Olymp_Project.WebApi
{
    [Route("animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private static List<Animal> _animalsStub = new List<Animal>
        {
            new Animal(0, new long[] {4, 3}, 30, 60, 90, "MALE", 1, 0, new long[] {1, 2}),
            new Animal(1, new long[] {3, 3}, 30, 60, 90, "MALE", 3, 0, new long[] {5, 3}),
            new Animal(2, new long[] {2, 3}, 30, 60, 90, "MALE", 1, 4, new long[] {2, 3}),
            new Animal(3, new long[] {1, 3}, 30, 60, 90, "MALE", 3, 3, new long[] {5, 3}),
            new Animal(4, new long[] {5, 4}, 30, 60, 90, "FEMALE", 1, 3, new long[] {4, 3}),
            new Animal(5, new long[] {8, 5}, 30, 60, 90, "FEMALE", 3, 3, new long[] {5, 5}),
            new Animal(6, new long[] {4, 7}, 30, 60, 90, "FEMALE", 4, 2, new long[] {5, 3}),
            new Animal(7, new long[] {5, 9}, 30, 60, 90, "FEMALE", 5, 2, new long[] {4, 6}),
            new Animal(8, new long[] {4, 8}, 30, 60, 90, "OTHER", 6, 2, new long[] {3, 3}),
            new Animal(9, new long[] {3, 5}, 30, 60, 90, "OTHER", 7, 0, new long[] {5, 8}),
            new Animal(10, new long[] {1, 4}, 30, 60, 90, "OTHER", 7, 0, new long[] {6, 3}),
        };


        [HttpGet("{animalId:int}")]
        public ActionResult<Animal> GetById(long? animalId)
        {
            if (!animalId.HasValue || animalId <= 0)
            {
                return BadRequest();
            }

            var animal = _animalsStub.Where(a => a.Id == animalId).OrderBy(a => a.Id).FirstOrDefault();
            // TODO: 401: Неверные авторизационные данные.
            if (animal == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(animal);
            }
        }
    }
}
