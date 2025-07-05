namespace ApiTemplate.GlobalExceptionHandler.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string msg) : base(msg) { }
    }
}
