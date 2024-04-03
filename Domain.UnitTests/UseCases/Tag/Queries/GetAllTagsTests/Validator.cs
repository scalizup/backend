using Application.Models;
using Application.Repositories;
using Application.Tags.Queries;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.Tag.Queries.GetAllTagsTests;

[TestClass]
public class Validator
{
    private readonly ITagGroupRepository _tagGroupRepository = Substitute.For<ITagGroupRepository>();
    private readonly GetAllTags.Validator _validator;

    private readonly PageQuery _pageQuery = new()
    {
        PageNumber = 1,
        PageSize = 10
    };

    public Validator()
    {
        _tagGroupRepository.GetTagGroupById(default, default)
            .ReturnsNullForAnyArgs();

        _validator = new(_tagGroupRepository);
    }

    [TestMethod]
    public async Task Id_GreaterThan()
    {
        // Arrange
        var command = new GetAllTags.Query(0, _pageQuery);

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