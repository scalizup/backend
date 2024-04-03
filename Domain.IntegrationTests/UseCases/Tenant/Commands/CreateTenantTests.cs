using Application.Tenants.Commands;

namespace Domain.IntegrationTests.UseCases.Tenant.Commands;

[TestClass]
public class CreateTenantTests : BaseIntegrationTest
{
    private readonly CreateTenant.Handler _handler = new(TenantRepository);

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