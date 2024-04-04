namespace Application.Common.Security;

public class AuthorizationRequestAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}