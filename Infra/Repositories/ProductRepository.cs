using Application.Models;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class ProductRepository(
    AppDbContext context) : IProductRepository
{
    public async Task<int> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        await context.Products.AddAsync(product, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Tags)
            .ThenInclude(t => t.TagGroup)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return product;
    }

    public async Task<PageQueryResponse<Product>> GetAllProductsAsync(int tenantId, PageQuery pageQuery, CancellationToken cancellationToken)
    {
        var skip = pageQuery.PageSize * (pageQuery.PageNumber - 1);

        var products = await context.Products
            .Where(p => p.TenantId == tenantId)
            .Include(p => p.Tags)
            .ThenInclude(t => t.TagGroup)
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(pageQuery.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntities = await context.Products
            .Where(p => p.TenantId == tenantId)
            .CountAsync(cancellationToken);

        return new PageQueryResponse<Product>(
            totalEntities,
            pageQuery,
            products);
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FindAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        context.Products.Remove(product);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        context.Products.Update(product);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<Product>> GetProductsByTagIds(IEnumerable<int> tagIds, CancellationToken cancellationToken)
    {
        var products = await context.Products
            .Include(p => p.Tags)
            .Where(p => p.Tags.Any(t => tagIds.Contains(t.Id)))
            .ToListAsync(cancellationToken);

        return products;
    }
}