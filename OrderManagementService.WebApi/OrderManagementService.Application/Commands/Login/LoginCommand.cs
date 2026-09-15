using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.Login
{
    public class LoginCommand : CommandBase<LoginCommandResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public override void Validate()
        {
            new LoginCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
