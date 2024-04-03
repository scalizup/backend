using Application.TagGroups.Commands;

namespace Domain.IntegrationTests.UseCases.TagGroup.Commands;

[TestClass]
public class CreateTagGroupTests : TenantAwareIntegrationTest
{
    private readonly CreateTagGroup.Handler _handler = new(TagGroupRepository);

    [TestMethod]
    public async Task Success()
    {
        // Arrange
        var command = new CreateTagGroup.Command(TenantId, "Tag Group 1");

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.Should().BeGreaterThan(0);
    }
}