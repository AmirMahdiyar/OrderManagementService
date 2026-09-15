namespace OrderManagementService.Application.Exceptions
{
    public class InputValidationFailedApplicationException : Domain.Exceptions.ApplicationExceptions.Base.ApplicationException
    {
        public InputValidationFailedApplicationException() : base("One of Your Input parameter is wrong !") { }
        public InputValidationFailedApplicationException(string message) : base(message)
        {

        }
    }
}
