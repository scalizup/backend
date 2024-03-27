namespace Domain.Exceptions;

public class NotFoundException(string s) : Exception(s);