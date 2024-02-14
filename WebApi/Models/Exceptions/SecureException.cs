namespace WebApi.Models.Exceptions
{
    public class SecureException : Exception
    {
        public SecureException() { }
        public SecureException(string message) : base(message) { }
        public SecureException(string message, Exception innerException) : base(message, innerException) { }
    }
}
