namespace OrderManagementService.Application.Exceptions
{
    public class NoChangesApplicationException : Domain.Exceptions.ApplicationExceptions.Base.ApplicationException
    {
        public NoChangesApplicationException() : base("No changes were saved.") { }
        public NoChangesApplicationException(string message) : base(message) { }
    }
}

