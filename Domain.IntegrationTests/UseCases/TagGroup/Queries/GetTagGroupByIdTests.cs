using Application.TagGroups.Queries;

namespace Domain.IntegrationTests.UseCases.TagGroup.Queries;

[TestClass]
public class GetTagGroupByIdTests : TenantAwareIntegrationTest
{
    private readonly GetTagGroupById.Handler _handler = new(TagGroupRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var expectedTagGroup = new Entities.TagGroup(TenantId, "Tools");

        var tagGroupId = await TagGroupRepository.CreateTagGroup(
            new Entities.TagGroup(expectedTagGroup.TenantId, expectedTagGroup.Name),
            default);

        var command = new GetTagGroupById.Query(tagGroupId);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Id.Should().Be(tagGroupId);
        handlerResult.Name.Should().Be(expectedTagGroup.Name);
    }
}
