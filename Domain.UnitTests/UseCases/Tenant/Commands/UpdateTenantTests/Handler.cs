using Domain.Exceptions;
using Domain.Repositories;
using Domain.UseCases.Tenant;
using Domain.UseCases.Tenant.Commands;
using NSubstitute;

namespace Domain.UnitTests.UseCases.Tenant.Commands.UpdateTenantTests;

[TestClass]
public class Handler
{
    private readonly UpdateTenant.Handler _handler;

    public Handler()
    {
        var tenantRepository = Substitute.For<ITenantRepository>();

        tenantRepository.GetTenantById(default, default)!
            .ReturnsForAnyArgs(Task.FromResult<Entities.Tenant>(null!));
        
        _handler = new(tenantRepository);
    }

    [TestMethod]
    public async Task UpdateNotExistingTenant_Throws()
    {
        // Arrange
        var command = new UpdateTenant.Command(0, null, null);
        
        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}