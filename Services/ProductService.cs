using Cosmatics.Models;
using Cosmatics.DTOs;
using Cosmatics.Data;

namespace Cosmatics.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepo;

    public ProductService(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepo.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _productRepo.GetByIdAsync(id);
    }

    public async Task<Product> CreateProductAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl
        };
        await _productRepo.AddAsync(product);
        return product;
    }

    public async Task UpdateProductAsync(int id, ProductUpdateDto dto)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product != null)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            await _productRepo.UpdateAsync(product);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product != null)
        {
            await _productRepo.DeleteAsync(product);
        }
    }
}
