using Application.Common.Exceptions;
using Application.Repositories;
using Application.Tags.Commands;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Domain.UnitTests.UseCases.Tag.Commands.UpdateTagTests;

[TestClass]
public class Handler
{
    private readonly UpdateTag.Handler _handler;

    public Handler()
    {
        var tagRepository = Substitute.For<ITagRepository>();

        tagRepository.GetTagById(default, default)!
            .ReturnsNull();

        _handler = new(tagRepository);
    }

    [TestMethod]
    public async Task UpdateNotExistingTag_Throws()
    {
        // Arrange
        var command = new UpdateTag.Command(0, null);

        // Act & Assert
        var act = () => _handler.Handle(command, default);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}