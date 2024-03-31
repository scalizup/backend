using Domain.UseCases.Tenant.Commands;
using FluentAssertions;

namespace Domain.IntegrationTests.UseCases.Tenant.Commands;

[TestClass]
public class UpdateTenantTests : BaseIntegrationTest
{
    private readonly UpdateTenant.Handler _handler = new(TenantRepository);

    [TestMethod]
    public async Task Update_Name()
    {
        // Arrange
        var expectedUpdatedTenant = new Entities.Tenant("Restaurant 2")
        {
            IsActive = false
        };

        var createdTenantId = await new CreateTenant.Handler(TenantRepository)
            .Handle(new CreateTenant.Command("Restaurant 1"), default);

        expectedUpdatedTenant.Id = createdTenantId;

        var command = new UpdateTenant.Command(expectedUpdatedTenant.Id, expectedUpdatedTenant.Name, null);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTenant = await TenantRepository.GetTenantById(createdTenantId, default);
        existingTenant.Should().BeEquivalentTo(expectedUpdatedTenant);
    }

    [TestMethod]
    public async Task UpdatesIsActive()
    {
        // Arrange
        var expectedUpdatedTenant = new Entities.Tenant("Restaurant 1")
        {
            IsActive = true
        };

        var createdTenantId = await new CreateTenant.Handler(TenantRepository)
            .Handle(new CreateTenant.Command("Restaurant 1"), default);

        expectedUpdatedTenant.Id = createdTenantId;
        
        var command = new UpdateTenant.Command(createdTenantId, null, true);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTenant = await TenantRepository.GetTenantById(createdTenantId, default);
        existingTenant.Should().BeEquivalentTo(expectedUpdatedTenant);
    }
}