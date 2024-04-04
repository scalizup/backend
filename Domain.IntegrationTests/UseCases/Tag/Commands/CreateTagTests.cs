using Application.UseCases.Tags.Commands;

namespace Domain.IntegrationTests.UseCases.Tag.Commands;

[TestClass]
public class CreateTagTests : TenantAwareIntegrationTest
{
    private readonly CreateTag.Handler _handler = new(TagRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var tagGroupId = await TagGroupRepository.CreateTagGroup(new Entities.TagGroup(TenantId, "Tools"), default);
        var command = new CreateTag.Command(TenantId, tagGroupId, "Hammer");

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeGreaterThan(0);
    }
}