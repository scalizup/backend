using Domain.Exceptions;
using Domain.Utils;

namespace Domain.UnitTests.Utils;

[TestClass]
public class PageQueryTests
{
    [TestMethod]
    public void InvalidPageNumber()
    {
        // Arrange
        var act = () => new PageQuery
        {
            PageNumber = -1,
            PageSize = 10
        };

        // Act & Assert
        act.Should().Throw<InvalidPageNumberException>();
    }

    [TestMethod]
    public void InvalidPageSize()
    {
        // Arrange
        var act = () => new PageQuery
        {
            PageNumber = 10,
            PageSize = -1
        };

        // Act & Assert
        act.Should().Throw<InvalidPageSizeException>();
    }
}