using Application.UseCases.TagGroups.Commands;

namespace Domain.IntegrationTests.UseCases.TagGroup.Commands;

[TestClass]
public class DeleteTagGroupTests : TenantAwareIntegrationTest
{
    private readonly DeleteTagGroup.Handler _handler = new(TagGroupRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var createdTagGroupId = await TagGroupRepository.CreateTagGroup(new(TenantId, "Tag Group 1"), default);

        var command = new DeleteTagGroup.Command(createdTagGroupId);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTagGroupId = await TagGroupRepository.GetTagGroupById(createdTagGroupId, default);
        existingTagGroupId.Should().BeNull();
    }
}