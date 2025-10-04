using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

namespace MyFirstApi.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class ProductsController : ControllerBase

    {

        [HttpGet]

        public ActionResult<List<string>> Get()

        {

            var products = new List<string> { "Apple", "Banana", "Orange" };
            return Ok(products);

        }

        [HttpPost]

        public ActionResult<string> Post([FromBody] string product)

        {

            return CreatedAtAction(nameof(Get), new { id = 1 }, product);

        }

        [HttpPut("{id}")]
        public ActionResult<string> Put(int id, [FromBody] string product)

        {

            return Ok($"Updated product {id} to: {product}");

        }

        [HttpDelete("{id:int}")]

        public ActionResult<string> Delete(int id)
        {
            return Ok($"Deleted product with id: {id}");
        }   

        }

    }

