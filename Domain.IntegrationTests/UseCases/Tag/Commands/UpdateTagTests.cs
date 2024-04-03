using Application.Tags.Commands;

namespace Domain.IntegrationTests.UseCases.Tag.Commands;

[TestClass]
public class UpdateTagTests : TenantAwareIntegrationTest
{
    private readonly UpdateTag.Handler _handler = new(TagRepository);

    [TestMethod]
    public async Task Update_Name()
    {
        // Arrange
        var tagGroupId = await TagGroupRepository.CreateTagGroup(new Entities.TagGroup(TenantId, "Tools"), default);

        var expectedUpdatedTag = new Entities.Tag(TenantId, tagGroupId, "Drill");
        await TagRepository.CreateTag(expectedUpdatedTag, default);

        var command = new UpdateTag.Command(expectedUpdatedTag.Id, "Hammer");

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTag = await TagRepository.GetTagById(expectedUpdatedTag.Id, default);
        existingTag.Should().BeEquivalentTo(expectedUpdatedTag);
    }
}