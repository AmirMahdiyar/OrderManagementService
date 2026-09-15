namespace OrderManagementService.Application.Commands.Login
{
    public class LoginCommandResponse
    {
        public string Token { get; private set; } = string.Empty;
        public bool Success { get; private set; }

        private LoginCommandResponse() { }

        public static LoginCommandResponse Response() => new();

        public LoginCommandResponse WithToken(string token)
        {
            Token = token;
            Success = true;
            return this;
        }
    }
}
