using Domain.Exceptions;
using Domain.Repositories;
using Domain.UseCases.Tenant;
using NSubstitute;

namespace Domain.UnitTests.UseCases.Tenant.DeleteTenantTests;

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