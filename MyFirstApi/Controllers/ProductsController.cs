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


        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
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

