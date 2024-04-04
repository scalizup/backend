using Application.Repositories;
using Application.UseCases.TagGroups.Queries;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.TagGroup.Queries.GetTagGroupByIdTests;

[TestClass]
public class Validator
{
    private readonly GetTagGroupById.Validator _validator;

    public Validator()
    {
        var tagGroupRepository = Substitute.For<ITagGroupRepository>();

        tagGroupRepository.GetTagGroupById(default, default)
            .ReturnsNullForAnyArgs();

        _validator = new GetTagGroupById.Validator(tagGroupRepository);
    }

    [TestMethod]
    public async Task Id_GreaterThanAndNotFound()
    {
        // Arrange
        var command = new GetTagGroupById.Query(0);
        
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
}