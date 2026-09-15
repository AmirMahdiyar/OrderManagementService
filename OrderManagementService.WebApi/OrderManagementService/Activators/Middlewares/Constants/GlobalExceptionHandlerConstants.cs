namespace OrderManagementService.Activators.Middlewares.Constants
{
    public static class GlobalExceptionHandlerConstants
    {
        public const string LogErrorMessage = "An error occurred while processing the request: {Message}";
        public const string HandledApplicationExceptionMessageTitle = "Something Went Wrong In Process";
        public const string HandledDomainExceptionMessageTitle = "Business Rule Violation";
        public const string HandledPresentationExceptionMessageTitle = "Something Went Wrong";
        public const string HandledValidationExceptionMessageTitle = "Validation Error";
        public const string HandledConcurrencyExceptionMessageTitle = "Concurrency Conflict";
        public const string ConcurrencyConflictErrorMessage = "The record was modified by another operation. Please refresh and try again.";
    }
}
