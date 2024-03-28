using Domain.UseCases.Tenant;
using Domain.UseCases.Tenant.Commands;

namespace Domain.UnitTests.UseCases.Tenant.Commands.CreateTenantTests;

[TestClass]
public class Validator
{
    private readonly CreateTenant.Validator _validator = new();

    [TestMethod]
    public async Task Name_EmptyNameAndMinimumLength()
    {
        // Arrange
        var command = new CreateTenant.Command("");
        
        // Act
        var validationResult = await _validator.ValidateAsync(command);
        
        // Assert
        Assert.IsFalse(validationResult.IsValid);

        var expectedErrorList = new List<string>
        {
            "'Name' must not be empty.",
            "The length of 'Name' must be at least 2 characters. You entered 0 characters."
        };

        validationResult.Errors
            .Select(e => e.ErrorMessage)
            .Should()
            .Equal(expectedErrorList);
    }
}