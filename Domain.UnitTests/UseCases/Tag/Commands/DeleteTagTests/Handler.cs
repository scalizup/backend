using Application.Common.Exceptions;
using Application.Repositories;
using Application.Tags.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.Tag.Commands.DeleteTagTests;

[TestClass]
public class Handler
{
    private readonly DeleteTag.Handler _handler;
    private readonly ITagRepository _tagGroupRepository = Substitute.For<ITagRepository>();

    public Handler()
    {
        _tagGroupRepository.GetTagById(default, default)!
            .ReturnsNull();

        _handler = new(_tagGroupRepository);
    }

    [TestMethod]
    public async Task DeleteNotExistingTenant_Throws()
    {
        // Arrange
        var command = new DeleteTag.Command(0);

        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}