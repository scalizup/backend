using Application.TagGroups.Commands;

namespace Domain.IntegrationTests.UseCases.TagGroup.Commands;

[TestClass]
public class UpdateTagGroupTests : TenantAwareIntegrationTest
{
    private readonly UpdateTagGroup.Handler _handler = new(TagGroupRepository);

    [TestMethod]
    public async Task Update_Name()
    {
        // Arrange
        var expectedUpdatedTagGroup = new Entities.TagGroup(TenantId, "Ingredients");
        await TagGroupRepository.CreateTagGroup(expectedUpdatedTagGroup, default);

        var command = new UpdateTagGroup.Command(expectedUpdatedTagGroup.Id, "Tools");

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeTrue();

        var existingTenant = await TagGroupRepository.GetTagGroupById(expectedUpdatedTagGroup.Id, default);
        existingTenant.Should().BeEquivalentTo(expectedUpdatedTagGroup);
    }
}