namespace Domain.IntegrationTests;

[TestClass] 
public class TenantAwareIntegrationTest : BaseIntegrationTest
{
    public int TenantId { get; private set; }

    [TestInitialize]
    public async Task TestInitialize()
    {
        TenantId = await CreateTenant();
    }
    
    public async Task<int> CreateTenant(string name = "Restaurant 1")
    {
        TenantId = await TenantRepository.CreateTenant(name, default);
        
        return TenantId;
    }
}