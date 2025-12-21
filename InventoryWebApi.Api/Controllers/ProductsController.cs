using InventoryWebApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Application.Models;
using System.Linq;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericService<Product> _genericService;
    private readonly IGenericService<ProductImage> _productImageService;
    private readonly IImageStorage _imageStorage;

    public ProductsController(IGenericService<Product> genericService,
        IGenericService<ProductImage> productImageService, 
        IImageStorage imageStorage)
    {
        _genericService = genericService;
        _productImageService = productImageService;
        _imageStorage = imageStorage;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var products = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _genericService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromForm] ProductCreateDto dto, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        var product = new Product
        {
            SkuId = dto.SkuId,
            ProductName = dto.ProductName,
            ShortDescription = dto.ShortDescription,
            Category = dto.Category,
            Uom = dto.Uom,
            IsActive = dto.IsActive,
            ProductMetadata = dto.ProductMetadata,
            IsBundled = dto.IsBundled,
            Rate = dto.Rate,
            Tax = dto.Tax,
        };

        if (dto.ImageFile is not null)
        {
            imageUrl = await _imageStorage.UploadAsync(dto.ImageFile, "products", cancellationToken);
            product.Images.Add(new ProductImage
            {
                ImageUrl = imageUrl,
                IsPrimary = true,
                SortOrder = 0
            });
            await _productImageService.CreateAsync(product.Images.First(), cancellationToken);
        }

        try
        {
            var created = await _genericService.CreateAsync(product, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch
        {
            // rollback uploaded file if persistence failed
            if (imageUrl is not null)
            {
                await _imageStorage.DeleteAsync(imageUrl, cancellationToken);
            }
            throw;
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and body ProductId must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();

        }

        string? newImageUrl = null;
        string? oldImageUrl = existing.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl;

        if (dto.ImageFile is not null)
        {
            newImageUrl = await _imageStorage.UploadAsync(dto.ImageFile, "products", cancellationToken);
            // unset existing primary
            var existingPrimary = existing.Images.FirstOrDefault(i => i.IsPrimary);
            if (existingPrimary is not null)
            {
                existingPrimary.IsPrimary = false;
                await _productImageService.UpdateAsync(existingPrimary, cancellationToken);
            }

            existing.Images.Add(new ProductImage
            {
                ImageUrl = newImageUrl,
                IsPrimary = true,
                SortOrder = 0
            });
            await _productImageService.UpdateAsync(existing.Images.First(), cancellationToken);
        }

        existing.SkuId = dto.SkuId;
        existing.ProductName = dto.ProductName;
        existing.ShortDescription = dto.ShortDescription;
        existing.Category = dto.Category;
        existing.Uom = dto.Uom;
        existing.IsActive = dto.IsActive;
        existing.ProductMetadata = dto.ProductMetadata;
        existing.IsBundled = dto.IsBundled;
        existing.Rate = dto.Rate;
        existing.Tax = dto.Tax;

        try
        {
            await _genericService.UpdateAsync(existing, cancellationToken);

            if (newImageUrl is not null && !string.IsNullOrEmpty(oldImageUrl))
            {
                // delete old image after successful DB update
                await _imageStorage.DeleteAsync(oldImageUrl, cancellationToken);
            }

            return NoContent();
        }
        catch
        {
            // if update failed, delete newly uploaded image
            if (newImageUrl is not null)
            {
                await _imageStorage.DeleteAsync(newImageUrl, cancellationToken);
            }
            throw;
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        await _genericService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
