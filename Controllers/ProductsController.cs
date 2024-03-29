﻿using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.AspNetCore.Http;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private DataContext context;

    public ProductsController(DataContext ctx)
    {
        context = ctx;
    }

    [HttpGet]
    public IAsyncEnumerable<Product> GetProducts()
    {
        return context.Products.AsAsyncEnumerable();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetProduct(long id)
    {
        Product? p = await context.Products.FindAsync(id);
        if (p == null) 
            return NotFound();
        
        return Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> SaveProduct(ProductBindingTarget target)
    {
        if (ModelState.IsValid)
        {
            Product p = target.ToProduct();
            await context.Products.AddAsync(p);
            await context.SaveChangesAsync();
            return Ok(p);
        }

        return BadRequest(ModelState);
    }

    [HttpPut]
    public async Task UpdateProduct(Product product)
    {
        context.Update(product);
        await context.SaveChangesAsync();
    }

    [HttpDelete("{id}")]
    public async Task DeleteProduct(long id)
    {
        context.Products.Remove(new Product() { ProductId = id });
        await context.SaveChangesAsync();
    }

    [HttpGet("redirect")]
    public IActionResult Redirect()
    {
        return RedirectToRoute(new
        {
            controller = "Products", action = "GetProducts"
        });
    }
}