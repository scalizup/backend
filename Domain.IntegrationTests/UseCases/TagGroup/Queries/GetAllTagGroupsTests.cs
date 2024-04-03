using Application.Models;
using Application.TagGroups.Queries;

namespace Domain.IntegrationTests.UseCases.TagGroup.Queries;

[TestClass]
public class GetAllTagGroupsTests : TenantAwareIntegrationTest
{
    private readonly GetAllTagGroups.Handler _handler = new(TagGroupRepository);

    private readonly PageQuery _pageQuery = new()
    {
        PageNumber = 1,
        PageSize = 10
    };
    
    [TestMethod]
    public async Task Success()
    {
        // Arrange
        await TagGroupRepository.CreateTagGroup(new Entities.TagGroup(TenantId, "Tools"), default);

        var command = new GetAllTagGroups.Query(TenantId, _pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(1);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(1);
        handlerResult.Items.Should().HaveCount(1);
    }
    
    [TestMethod]
    public async Task TenantIdFilter()
    {
        // Arrange
        await TagGroupRepository.CreateTagGroup(new Entities.TagGroup(TenantId, "Tools"), default);
        
        var defaultTenantIdCommand = new GetAllTagGroups.Query(TenantId, _pageQuery);
        var differentTenantIdCommand = new GetAllTagGroups.Query(await CreateTenant("Restaurant 2"), _pageQuery);

        // Act
        var handlerResult = new
        {
            defaultTenantId = await _handler.Handle(defaultTenantIdCommand, default),
            differentTenantId = await _handler.Handle(differentTenantIdCommand, default),
        };

        // Assert
        // Empty result for different tenant id
        handlerResult.defaultTenantId.TotalPages.Should().Be(1);
        handlerResult.defaultTenantId.CurrentPage.Should().Be(1);
        handlerResult.defaultTenantId.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.defaultTenantId.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.defaultTenantId.TotalItems.Should().Be(1);
        handlerResult.defaultTenantId.Items.Should().HaveCount(1);
        
        // Filled result for default tenant id
        handlerResult.differentTenantId.TotalPages.Should().Be(0);
        handlerResult.differentTenantId.CurrentPage.Should().Be(1);
        handlerResult.differentTenantId.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.differentTenantId.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.differentTenantId.TotalItems.Should().Be(0);
        handlerResult.differentTenantId.Items.Should().HaveCount(0);
    }

    [TestMethod]
    public async Task Empty()
    {
        // Arrange
        var command = new GetAllTagGroups.Query(TenantId, _pageQuery);

        // Act
        var handlerResult = await _handler.Handle(command, default);

        // Assert
        handlerResult.TotalPages.Should().Be(0);
        handlerResult.CurrentPage.Should().Be(1);
        handlerResult.PageSize.Should().Be(_pageQuery.PageSize);
        handlerResult.CurrentPage.Should().Be(_pageQuery.PageNumber);
        handlerResult.TotalItems.Should().Be(0);
        handlerResult.Items.Should().HaveCount(0);
    }
}