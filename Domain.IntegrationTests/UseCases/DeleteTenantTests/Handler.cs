using Domain.Exceptions;
using Domain.UseCases.Tenant;
using FluentAssertions;

namespace Domain.IntegrationTests.UseCases.DeleteTenantTests;

[TestClass]
public class Handler : BaseIntegrationTest
{
    private readonly DeleteTenant.Handler _handler = new(TenantRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var createdTenantId = await TenantRepository.CreateTenant("Restaurant 1", default);

        var command = new DeleteTenant.Command(createdTenantId);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTenant = await TenantRepository.GetTenantById(createdTenantId, default);
        existingTenant.Should().BeNull();
    }

    [TestMethod]
    public async Task ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteTenant.Command(1);

        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}