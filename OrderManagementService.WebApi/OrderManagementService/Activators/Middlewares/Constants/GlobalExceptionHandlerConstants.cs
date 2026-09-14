namespace OrderManagementService.Activators.Middlewares.Constants
{
    public static class GlobalExceptionHandlerConstants
    {
        public const string LogErrorMessage = "An error occurred while processing the request: {Message}";
        public const string HandledApplicationExceptionMessageTitle = "Something Went Wrong In Process";
        public const string HandledDomainExceptionMessageTitle = "Business Rule Violation";
        public const string HandledPresentationExceptionMessageTitle = "Something Went Wrong";
    }
}
