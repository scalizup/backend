using System.Text.Json.Serialization.Metadata;
using Application.Common.Interfaces;
using Presentation.API.Services;

namespace Presentation.API;

public static class TypeResolvers
{
    public static Action<JsonTypeInfo> GetBudgetsTypeResolvers()
    {
        return typeInfo =>
        {
            if (typeInfo.Type == typeof(IUser))
                typeInfo.CreateObject = () => new CurrentUser(null!);
        };
    }
}