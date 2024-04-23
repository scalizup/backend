using Application.UseCases.Menu.Queries;
using Application.UseCases.MenuSort.Commands;
using Application.UseCases.Tenants.Commands;
using Domain.Entities;

namespace Domain.IntegrationTests.UseCases.MenuSort.Commands;

[TestClass]
public class CreateMenuSortTest : BaseIntegrationTest
{
    private readonly CreateMenuSort.Handler _handler = new(MenuSortRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var tenantId = await TenantRepository.CreateTenant("Tenant Test", default);

        var mockTagGroups = new List<TagGroup>()
        {
            new(tenantId, "Course Type") { Id = 1 },
            new(tenantId, "Cuisine") { Id = 2 },
            new(tenantId, "Allergens") { Id = 3 },
        };

        foreach (var tagGroup in mockTagGroups)
        {
            await TagGroupRepository.CreateTagGroup(tagGroup, default);
        }

        var mockTags = new List<Tag>
        {
            new(tenantId, 1, "Appetizers") { Id = 1 },
            new(tenantId, 1, "Main Courses") { Id = 2 },
            new(tenantId, 1, "Desserts") { Id = 3 },
            new(tenantId, 2, "Italian") { Id = 4 },
            new(tenantId, 2, "Mexican") { Id = 5 },
            new(tenantId, 2, "Asian") { Id = 6 },
            new(tenantId, 3, "Gluten-Free") { Id = 7 },
            new(tenantId, 3, "Dairy-Free") { Id = 8 },
            new(tenantId, 3, "Vegan") { Id = 9 },
        };

        foreach (var tag in mockTags)
        {
            await TagRepository.CreateTag(tag, default);
        }
        
        var mockProducts = new List<Product>
        {
            new(tenantId, "Margherita Pizza") { Id = 1, TagIds = new List<int> { 1, 4 } },
            new(tenantId, "Coq au Vin") { Id = 2, TagIds = new List<int> { 2, 4 } },
            new(tenantId, "Sushi Platter") { Id = 3, TagIds = new List<int> { 3, 6 } },
            new(tenantId, "Tiramisu") { Id = 4, TagIds = new List<int> { 3, 4, 7 } },
            new(tenantId, "Seafood Risotto") { Id = 5, TagIds = new List<int> { 2, 6, 8 } },
            new(tenantId, "Cheese Board") { Id = 6, TagIds = new List<int> { 1, 4, 8 } },
            new(tenantId, "Beef Wellington") { Id = 7, TagIds = new List<int> { 2, 4 } },
            new(tenantId, "Crème Brûlée") { Id = 8, TagIds = new List<int> { 3, 4, 7, 9 } },
            new(tenantId, "Miso Soup") { Id = 9, TagIds = new List<int> { 1, 6 } },
            new(tenantId, "Caprese Salad") { Id = 10, TagIds = new List<int> { 1, 4, 7, 9 } },
        };
        
        foreach (var product in mockProducts)
        {
            await ProductRepository.CreateProductAsync(product, default);
        }

        var menuOrders = new List<CreateMenuSort.OrderDto>
        {
            new(2, new[] { 2, 7, 5 }),
            new(1, new[] { 6, 1, 7 }),
            new(3, new[] { 4, 8, 3 }),
        };

        var command = new CreateMenuSort.Command(1, menuOrders)
        {
            TenantId = tenantId
        };

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeGreaterThan(0);

        var menuSort = new GetMenuSort.Query
        {
            TenantId = tenantId
        };
        
        var menuSortResult = await new GetMenuSort.Handler(
                MenuSortRepository,
                TagGroupRepository,
                TagRepository,
                ProductRepository)
            .Handle(menuSort, default);
    }
}