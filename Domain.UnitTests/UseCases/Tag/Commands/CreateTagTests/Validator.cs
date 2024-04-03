using Application.Repositories;
using Application.Tags.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.Tag.Commands.CreateTagTests;

[TestClass]
public class Validator
{
    private readonly CreateTag.Validator _validator;
    private readonly ITenantRepository _tenantRepositoryMock = Substitute.For<ITenantRepository>();
    private readonly ITagGroupRepository _tagGroupRepository = Substitute.For<ITagGroupRepository>();

    public Validator()
    {
        _tenantRepositoryMock
            .GetTenantById(default, default)
            .ReturnsForAnyArgs(new Entities.Tenant("Tenant"));

        _tagGroupRepository
            .GetTagGroupById(default, default)
            .ReturnsForAnyArgs(new Entities.TagGroup(1, "Tag Group"));

        _validator = new(_tenantRepositoryMock, _tagGroupRepository);
    }

    [TestMethod]
    public async Task TenantId_GreaterThanAndNotFound()
    {
        // Arrange
        var command = new CreateTag.Command(0, 1, "Tag");

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
    public async Task TagGroupId_GreaterThanAndNotFound()
    {
        // Arrange
        var command = new CreateTag.Command(1, 0, "Tag");

        _tagGroupRepository
            .GetTagGroupById(default, default)
            .ReturnsNull();

        // Act
        var validationResult = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsFalse(validationResult.IsValid);

        var expectedErrorList = new List<string>
        {
            "'Tag Group Id' must be greater than '0'.",
            "Tag Group with id '0' does not exist."
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
        var command = new CreateTag.Command(1, 1, "");

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