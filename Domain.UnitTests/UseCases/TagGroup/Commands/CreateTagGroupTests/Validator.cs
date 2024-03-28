using Domain.Repositories;
using Domain.UseCases.TagGroup;
using Domain.UseCases.TagGroup.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.TagGroup.Commands.CreateTagGroupTests;

[TestClass]
public class Validator
{
    private readonly CreateTagGroup.Validator _validator;
    private readonly ITenantRepository _tenantRepositoryMock = Substitute.For<ITenantRepository>();

    public Validator()
    {
        _tenantRepositoryMock
            .GetTenantById(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Entities.Tenant("Tenant"));

        _validator = new(_tenantRepositoryMock);
    }

    [TestMethod]
    public async Task TenantId_GreaterThanAndNotFound()
    {
        // Arrange
        var command = new CreateTagGroup.Command(0, "Tag Group");
        
        _tenantRepositoryMock
            .GetTenantById(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var validationResult = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsFalse(validationResult.IsValid);

        var expectedErrorList = new List<string>
        {
            "'Tenant Id' must be greater than '0'.",
            "Tenant with id '0' does not exist."
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
        var command = new CreateTagGroup.Command(1, "");

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