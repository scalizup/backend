using Application.Models;
using Application.Tenants.Queries;

namespace Domain.IntegrationTests.UseCases.Tenant.Queries;

[TestClass]
public class GetAllTenantTests : BaseIntegrationTest
{
    private readonly GetAllTenants.Handler _handler = new(TenantRepository);

    private PageQuery _pageQuery = new()
    {
        PageNumber = 1,
        PageSize = 10
    };
    
    [TestMethod]
    public async Task Success()
    {
        // Arrange
        await TenantRepository.CreateTenant("Tenant 1", default);
       
        var command = new GetAllTenants.Query(_pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(1);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(1);
        handlerResult.Items.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task Empty()
    {
        // Arrange
        var command = new GetAllTenants.Query(_pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(0);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(0);
        handlerResult.Items.Should().HaveCount(0);
    }
}