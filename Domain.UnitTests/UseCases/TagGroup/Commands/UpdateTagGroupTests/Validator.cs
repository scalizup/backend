using Domain.UseCases.TagGroup.Commands;

namespace Domain.UnitTests.UseCases.TagGroup.Commands.UpdateTagGroupTests;

[TestClass]
public class Validator
{
    private readonly UpdateTagGroup.Validator _validator = new();

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new UpdateTagGroup.Command(0, null);
        
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
    public async Task Name_WhenNotNull_NotEmptyAndMinimumLength()
    {
        // Arrange
        var command = new UpdateTagGroup.Command(1, "");

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