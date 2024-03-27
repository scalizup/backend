using Domain.Exceptions;
using Domain.Utils;

namespace Domain.UnitTests.Utils;

public class PageQueryTests
{
    [TestMethod]
    public void InvalidPageNumber()
    {
        // Arrange
        var act = () => new PageQuery(-1, 1);

        // Act & Assert
        act.Should().Throw<InvalidPageNumberException>();
    }

    [TestMethod]
    public void InvalidPageSize()
    {
        // Arrange
        var act = () => new PageQuery(1, -1);

        // Act & Assert
        act.Should().Throw<InvalidPageSizeException>();
    }
}