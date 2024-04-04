using Application.UseCases.Tags.Commands;

namespace Domain.UnitTests.UseCases.Tag.Commands.DeleteTagTests;

[TestClass]
public class Validator
{
    private readonly DeleteTag.Validator _validator = new();

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new DeleteTag.Command(0);

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