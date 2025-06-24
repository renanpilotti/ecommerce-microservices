namespace BuildingBlocks.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string name, object key) : base($"{name} ({key}) was not found") { }
    }

    public class BadRequestException(string message) : Exception(message)
    {
    }

    public class ConflictException(string message) : Exception(message)
    {
    }

    public class ForbiddenException(string message) : Exception(message)
    {
    }

    public class TooManyRequestsException(string message) : Exception(message)
    {
    }

    public class ServiceUnavailableException(string message) : Exception(message)
    {
    }
}
