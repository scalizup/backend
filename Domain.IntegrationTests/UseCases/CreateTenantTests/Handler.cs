using Domain.UseCases.Tenant;
using FluentAssertions;

namespace Domain.IntegrationTests.UseCases.CreateTenantTests;

[TestClass]
public class Handler : BaseIntegrationTest
{
    private readonly CreateTenant.Handler _handler;
    
    public Handler()
    {
        _handler = new CreateTenant.Handler(TenantRepository);
    }

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var command = new CreateTenant.Command("Restaurant 1");

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeGreaterThan(0);
    }
}