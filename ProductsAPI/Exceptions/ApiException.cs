namespace ProductsAPI.API.Exceptions
{
    public abstract class ApiException(int statusCode, string message) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }

    public class UnauthorizedException(string message = "Usuário ou senha inválidos.")
        : ApiException(401, message);

    public class BadRequestException(string message)
        : ApiException(400, message);

    public class ConflictException(string message)
        : ApiException(409, message);
}
