using Application.UseCases.Tenants.Queries;

namespace Domain.UnitTests.UseCases.Tenant.Queries.GetTenantByIdTests;

[TestClass]
public class Validator
{
    private readonly GetTenantById.Validator _validator = new();

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new GetTenantById.Query(0);
        
        // Act
        var validationResult = await _validator.ValidateAsync(command);
        
        // Assert
        Assert.IsFalse(validationResult.IsValid);

        var expectedErrorList = new List<string>
        {
            "'Id' must be greater than '0'.",
        };

        validationResult.Errors
            .Select(e => e.ErrorMessage)
            .Should()
            .Equal(expectedErrorList);
    }
}