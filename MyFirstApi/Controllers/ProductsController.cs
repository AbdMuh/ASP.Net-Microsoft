using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{

    [ApiController]

    [Route("api/[controller]")]

    public class ProductsController : ControllerBase

    {
        public static List<Product> Products = new List<Product>
        {
            new Product { Id = 1, Name = "Apple Juice", Price = 2.5M },
            new Product { Id = 2, Name = "Banana Juice", Price = 2.0M },
            new Product { Id = 3, Name = "Orange Juice", Price = 3.0M }
        };
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ILogger<ProductsController> logger)
        //No need to register ILogger in DI, it's built-in, automatic DI support
        {
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            if(!Products.Any())
            {
                return NotFound();
            }
            _logger.LogInformation("Fetched all products, count: {Count}", Products.Count);
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            if (id < 0)
            {
                // return BadRequest("Invalid product ID."); not an exception, just an error response
                throw new ArgumentException("Invalid product ID."); //exception forcefully thrown
            }
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("No Product found with the given ID.");
            }
            _logger.LogInformation("Fetched product details, Id: {Id}", id);
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product product)
        {
            product.Id = Products.Max(p => p.Id) + 1;
            Products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            //[FromBody], automatically deserializes the JSON payload from the request body into a Product C# object.
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Products.Remove(product);
            return NoContent();
        }
    }

    }

