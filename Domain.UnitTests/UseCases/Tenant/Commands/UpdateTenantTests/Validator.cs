using Application.UseCases.Tenants.Commands;

namespace Domain.UnitTests.UseCases.Tenant.Commands.UpdateTenantTests;

[TestClass]
public class Validator
{
    private readonly UpdateTenant.Validator _validator = new();

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new UpdateTenant.Command(0, null!, null!);
        
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
    
    [TestMethod]
    public async Task Name_EmptyNameAndMinimumLength()
    {
        // Arrange
        var command = new UpdateTenant.Command(1, "", null!);

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