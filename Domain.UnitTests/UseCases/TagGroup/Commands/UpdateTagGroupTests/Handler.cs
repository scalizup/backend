using Domain.Exceptions;
using Domain.Repositories;
using Domain.UseCases.TagGroup.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.TagGroup.Commands.UpdateTagGroupTests;

[TestClass]
public class Handler
{
    private readonly UpdateTagGroup.Handler _handler;

    public Handler()
    {
        var tenantRepository = Substitute.For<ITagGroupRepository>();

        tenantRepository.GetTagGroupById(default, default)!
            .ReturnsNull();
        
        _handler = new(tenantRepository);
    }

    [TestMethod]
    public async Task UpdateNotExistingTagGroup_Throws()
    {
        // Arrange
        var command = new UpdateTagGroup.Command(0, null);
        
        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}