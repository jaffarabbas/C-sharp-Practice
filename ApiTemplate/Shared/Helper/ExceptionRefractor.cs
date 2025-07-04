using ApiTemplate.Dtos;

namespace EcommerceAppBackend.Helper;

public class ExceptionRefractor
{
    public static ExceptionMessage ExceptionMessage(Exception exception)
    {
        return new ExceptionMessage()
        {
            message = exception.Message,
        };
    }
}