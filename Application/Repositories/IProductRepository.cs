using System.Collections;
using Application.Models;
using Domain.Entities;

namespace Application.Repositories;

public interface IProductRepository
{
    Task<int> CreateProductAsync(Product product, CancellationToken cancellationToken);
    
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    
    Task<PageQueryResponse<Product>> GetAllProductsAsync(
        int tenantId,
        PageQuery pageQuery,
        CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> productIds, CancellationToken cancellationToken);
    
    Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);

    Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken);

    Task<IEnumerable<Product>> GetProductsByTagIds(IEnumerable<int> tagIds, CancellationToken cancellationToken);
}