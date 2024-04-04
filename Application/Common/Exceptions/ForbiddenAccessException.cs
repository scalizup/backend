namespace Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string adminsCannotBeAddedToRoles) : base(adminsCannotBeAddedToRoles) { }
    
    public ForbiddenAccessException() : base() { }
}
