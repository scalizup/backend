using Domain.UseCases.TagGroup;
using Domain.UseCases.TagGroup.Commands;

namespace Domain.UnitTests.UseCases.TagGroup.Commands.DeleteTagGroupTests;

[TestClass]
public class Validator
{
    private readonly DeleteTagGroup.Validator _validator = new();

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new DeleteTagGroup.Command(0);
        
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