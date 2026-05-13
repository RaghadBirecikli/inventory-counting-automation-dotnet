using InventoryCounting.Domain.Entities;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products
            .Where(product => product.IsActive)
            .OrderBy(product => product.Code)
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id && product.IsActive);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Code))
        {
            ModelState.AddModelError(nameof(product.Code), "Code is required.");
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            ModelState.AddModelError(nameof(product.Name), "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(product.Unit))
        {
            ModelState.AddModelError(nameof(product.Unit), "Unit is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var codeExists = await _context.Products
            .AnyAsync(existingProduct => existingProduct.Code == product.Code);

        if (codeExists)
        {
            ModelState.AddModelError(nameof(product.Code), "Product code must be unique.");
            return ValidationProblem(ModelState);
        }

        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(currentProduct => currentProduct.Id == id && currentProduct.IsActive);

        if (existingProduct is null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Unit = product.Unit;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(currentProduct => currentProduct.Id == id && currentProduct.IsActive);

        if (product is null)
        {
            return NotFound();
        }

        product.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
