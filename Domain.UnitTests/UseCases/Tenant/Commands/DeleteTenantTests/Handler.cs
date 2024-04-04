using Application.Common.Exceptions;
using Application.Repositories;
using Application.UseCases.Tenants.Commands;
using NSubstitute;

namespace Domain.UnitTests.UseCases.Tenant.Commands.DeleteTenantTests;

[TestClass]
public class Handler
{
    private readonly DeleteTenant.Handler _handler;

    public Handler()
    {
        var tenantRepository = Substitute.For<ITenantRepository>();

        tenantRepository.GetTenantById(default, default)!
            .ReturnsForAnyArgs(Task.FromResult<Entities.Tenant>(null!));
        
        _handler = new(tenantRepository);
    }

    [TestMethod]
    public async Task DeleteNotExistingTenant_Throws()
    {
        // Arrange
        var command = new DeleteTenant.Command(0);
        
        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}