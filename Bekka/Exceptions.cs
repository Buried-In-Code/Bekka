namespace Bekka;

[Serializable]
public class ServiceException : Exception
{
    public ServiceException() { }
    public ServiceException(string message) : base(message) { }
    public ServiceException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class AuthenticationException : ServiceException
{
    public AuthenticationException() { }
    public AuthenticationException(string message) : base(message) { }
    public AuthenticationException(string message, Exception inner) : base(message, inner) { }
}