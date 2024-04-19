// namespace TestProject1;
//
// public interface ICondition
// {
//     IEnumerable<object> ApplyCondition(EntityProperty entityProperty, IEnumerable<object> entities);
// }
//
//  public class IntCondition : ICondition
//     {
//         public IEnumerable<object> ApplyCondition(EntityProperty entityProperty, IEnumerable<object> entities)
//         {
//             if (entities == null || !entities.Any())
//             {
//                 return Enumerable.Empty<object>(); // Return empty collection if input is null or empty
//             }
//
//             var nestedProperties = entityProperty.PropertyName.Split('.');
//             var currentEntities = entities;
//
//             // Traverse through each nested property
//             foreach (var propertyName in nestedProperties)
//             {
//                 // Get the property info for the current level
//                 var propertyInfo = currentEntities.FirstOrDefault()?.GetType().GetProperty(propertyName);
//
//                 if (propertyInfo == null)
//                 {
//                     throw new Exception($"Property '{propertyName}' not found or sequence is empty");
//                 }
//
//                 // Update current entities based on the property value
//                 currentEntities = currentEntities.Select(x => propertyInfo.GetValue(x)).OfType<IEnumerable<object>>().SelectMany(x => x);
//
//                 if (!currentEntities.Any())
//                 {
//                     return Enumerable.Empty<object>(); // Return empty collection if no elements in currentEntities
//                 }
//             }
//
//             // Now currentEntities should contain the final set of entities to filter
//             var finalProperty = currentEntities.FirstOrDefault()?.GetType().GetProperty(nestedProperties.Last());
//
//             if (finalProperty == null)
//             {
//                 throw new Exception($"Property '{nestedProperties.Last()}' not found or sequence is empty");
//             }
//
//             var rawEntities = currentEntities.ToList();
//
//             if (entityProperty.PropertyDataTypeRules.TryGetValue("Where", out var where))
//             {
//                 var value = int.Parse(entityProperty.PropertyDataTypeRules["WhereValue"]);
//
//                 if (where == "Greater")
//                 {
//                     rawEntities = rawEntities.Where(x => (int)finalProperty.GetValue(x) > value).ToList();
//                 }
//                 
//                 if (where == "Less")
//                 {
//                     rawEntities = rawEntities.Where(x => (int)finalProperty.GetValue(x) < value).ToList();
//                 }
//             }
//             
//             if (entityProperty.PropertyDataTypeRules.TryGetValue("OrderBy", out var orderBy))
//             {
//                 if (orderBy == "Asc")
//                 {
//                     rawEntities = rawEntities.OrderBy(x => finalProperty.GetValue(x)).ToList();
//                 }
//
//                 if (orderBy == "Desc")
//                 {
//                     rawEntities = rawEntities.OrderByDescending(x => finalProperty.GetValue(x)).ToList();
//                 }
//             }
//
//             return rawEntities;
//         }
//     }
// public class StringCondition : ICondition
// {
//     public IEnumerable<object> ApplyCondition(EntityProperty entityProperty, IEnumerable<object> entities)
//     {
//         var property = entities.First().GetType().GetProperty(entityProperty.PropertyName);
//
//         var rawEntities = entities.ToList();
//
//         if (property == null)
//         {
//             throw new Exception("Property not found");
//         }
//
//         if (entityProperty.PropertyDataTypeRules.TryGetValue("OrderBy", out var orderBy))
//         {
//             if (orderBy == "Asc")
//             {
//                 rawEntities = rawEntities.OrderBy(x => property.GetValue(x)).ToList();
//             }
//
//             if (orderBy == "Desc")
//             {
//                 rawEntities = rawEntities.OrderByDescending(x => property.GetValue(x)).ToList();
//             }
//         }
//
//         return rawEntities;
//     }
// }
//
// public class Rule
// {
//     public List<EntityProperty> EntityProperties { get; set; }
//
//     public IEnumerable<object> CalculateConditions(IEnumerable<object> entities)
//     {
//         foreach (var entityProperty in EntityProperties)
//         {
//             entities = ApplyCondition(entityProperty, entities);
//         }
//
//         return entities;
//     }
//
//     private IEnumerable<object> ApplyCondition(EntityProperty entityProperty, IEnumerable<object> entities)
//     {
//         ICondition condition = entityProperty.DataType switch
//         {
//             EntityProperty.PropertyDataType.Int => new IntCondition(),
//             EntityProperty.PropertyDataType.String => new StringCondition(),
//             _ => throw new Exception("Invalid data type")
//         };
//
//         return condition.ApplyCondition(entityProperty, entities);
//     }
// }
//
// public class EntityProperty
// {
//     public enum PropertyDataType
//     {
//         String,
//         Int
//     }
//
//     public PropertyDataType DataType { get; set; }
//
//     public Dictionary<string, string> PropertyDataTypeRules { get; set; }
//
//     public string PropertyName { get; set; }
// }