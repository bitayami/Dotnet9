namespace Dotnet9.Middleware
{
    public class ForbiddenException : Exception
    {
        
        public ForbiddenException(string? message) : base(message)
        {
        }
    }
}
