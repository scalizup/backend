using Newtonsoft.Json;
using RulesEngine.Extensions;
using RulesEngine.Models;

namespace TestProject1;

public class PropertyOrder
{
    public int TenantId { get; set; }

    public string EntityName { get; set; }
    
    public List<int> OrderBy { get; set; }
}

[TestClass]
public class UnitTest1
{
    private List<PropertyOrder> _mockedPropertyOrders = new()
    {
        new PropertyOrder
        {
            EntityName = "Tag",
            OrderBy = new List<int> { 3, 2, 1, 4 }
        }
    };
    
    [TestMethod]
    public async Task OrderDynamically()
    {
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "Sobremesa" },
            new() { Id = 2, Name = "Prato Principal" },
            new() { Id = 3, Name = "Entradas" },
            new() { Id = 4, Name = "Bebidas" }
        }; 
        
        
        var tagOrderIndex = _mockedPropertyOrders
            .First(po => po.EntityName == "Tag")
            .OrderBy
            .Select((id, index) => new { id, index })
            .ToDictionary(x => x.id, x => x.index);

        var result = tags.OrderBy(t => tagOrderIndex[t.Id]).ToList();
        
        // Do the result for EF Core
        
        
        
        
        
        
        
    }
}

public class Tag
{
    public int Id { get; set; }
    
    public string Name { get; set; }
}