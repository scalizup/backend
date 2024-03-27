using Domain.Exceptions;
using Domain.UseCases.Tenant;
using Domain.Utils;
using FluentAssertions;

namespace Domain.IntegrationTests.UseCases.GetAllTenantTests;

[TestClass]
public class Handler : BaseIntegrationTest
{
    private readonly GetAllTenants.Handler _handler = new(TenantRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        await TenantRepository.CreateTenant("Tenant 1", default);
        
        var pageQuery = new PageQuery();
        var command = new GetAllTenants.Query(pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(1);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(1);
        handlerResult.Items.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task Empty()
    {
        // Arrange
        var pageQuery = new PageQuery();
        var command = new GetAllTenants.Query(pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(0);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(0);
        handlerResult.Items.Should().HaveCount(0);
    }
}