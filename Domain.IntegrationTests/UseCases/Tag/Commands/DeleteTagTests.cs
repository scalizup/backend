using Domain.UseCases.Tag.Commands;
using FluentAssertions;

namespace Domain.IntegrationTests.UseCases.Tag.Commands;

[TestClass]
public class DeleteTagTests : TenantAwareIntegrationTest
{
    private readonly DeleteTag.Handler _handler = new(TagRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var createdTagGroupId = await TagGroupRepository.CreateTagGroup(new(TenantId, "Tag Group 1"), default);
        var createdTag = new Entities.Tag(TenantId, createdTagGroupId, "Tag 1");
        
        await TagRepository.CreateTag(createdTag, default);

        var command = new DeleteTag.Command(createdTag.Id);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTagId = await TagRepository.GetTagById(createdTag.Id, default);
        existingTagId.Should().BeNull();
    }
}