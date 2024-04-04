using Application.Common.Exceptions;
using Application.Repositories;
using Application.UseCases.TagGroups.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.TagGroup.Commands.DeleteTagGroupTests;

[TestClass]
public class Handler
{
    private readonly DeleteTagGroup.Handler _handler;
    private readonly ITagGroupRepository _tagGroupRepository = Substitute.For<ITagGroupRepository>();

    public Handler()
    {
        _tagGroupRepository.GetTagGroupById(default, default)!
            .ReturnsNull();

        _handler = new(_tagGroupRepository);
    }

    [TestMethod]
    public async Task DeleteNotExistingTenant_Throws()
    {
        // Arrange
        var command = new DeleteTagGroup.Command(0);

        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}