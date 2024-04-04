namespace Application.Common.Security;

/// <summary>
/// Specifies the class this attribute is applied to requires authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class. 
    /// </summary>
    public AuthorizeAttribute()
    {
    }

    /// <summary>
    /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
    /// </summary>
    public string Role { get; set; } = default!;

    private string[] _roles = Array.Empty<string>();
    public string[] Roles
    {
        get
        {
            if (!string.IsNullOrEmpty(Role))
            {
                _roles = [Role];
            }

            return _roles;
        }
        set => _roles = value;
    }

    /// <summary>
    /// Gets or sets the policy name that determines access to the resource.
    /// </summary>
    public string Policy { get; set; } = default!;
}