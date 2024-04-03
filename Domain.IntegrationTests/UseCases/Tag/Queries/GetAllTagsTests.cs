using Application.Models;
using Application.Tags.Queries;

namespace Domain.IntegrationTests.UseCases.Tag.Queries;

[TestClass]
public class GetAllTagsTests : TenantAwareIntegrationTest
{
    private readonly GetAllTags.Handler _handler = new(TagRepository);

    private readonly PageQuery _pageQuery = new()
    {
        PageNumber = 1,
        PageSize = 10
    };

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var tagGroupId = await TagGroupRepository.CreateTagGroup(new Entities.TagGroup(TenantId, "Tools"), default);
        await TagRepository.CreateTag(new Entities.Tag(TenantId, tagGroupId, "Hammer"), default);

        var command = new GetAllTags.Query(tagGroupId, _pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(1);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(1);
        handlerResult.Items.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task Empty()
    {
        // Arrange
        var command = new GetAllTags.Query(TenantId, _pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(0);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(0);
        handlerResult.Items.Should().HaveCount(0);
    }
}