using Domain.Repositories;
using Domain.UseCases.User.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.User.Commands.LoginUserTests;

[TestClass]
public class Validator
{
    private readonly LoginUser.Validator _validator;
    
    public Validator()
    {
        var userRepository = Substitute.For<IUserRepository>();

        userRepository.GetByUsernameAsync(default!, default)
            .ReturnsNullForAnyArgs();

        _validator = new LoginUser.Validator(userRepository);
    }

    [TestMethod]
    public async Task AllValidations()
    {
        // Arrange
        var command = new LoginUser.Command("", "");
        
        // Act
        var validationResult = await _validator.ValidateAsync(command);
        
        // Assert
        Assert.IsFalse(validationResult.IsValid);

        var expectedErrorList = new List<string>
        {
            "'Username' must not be empty.",
            "The length of 'Username' must be at least 2 characters. You entered 0 characters.",
            "User not found or is inactive.",
            "'Password' must not be empty.",
            "The length of 'Password' must be at least 6 characters. You entered 0 characters.",
        };

        validationResult.Errors
            .Select(e => e.ErrorMessage)
            .Should()
            .Equal(expectedErrorList);
    }
}