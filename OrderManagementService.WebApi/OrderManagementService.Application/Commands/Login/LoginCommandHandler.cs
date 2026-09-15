using MediatR;
using OrderManagementService.Application.Contracts.Jwt;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Services.DomainServices.UserPassword;

namespace OrderManagementService.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IUserPasswordDomainService _userPasswordDomainService;
        private readonly IJwtProvider _jwtProvider;

        public LoginCommandHandler(
            IUserQueryRepository userRepository,
            IUserPasswordDomainService userPasswordDomainService,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _userPasswordDomainService = userPasswordDomainService;
            _jwtProvider = jwtProvider;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.User user = await GetUser(request, cancellationToken);

            var token = _jwtProvider.GenerateToken(user);

            return LoginCommandResponse
                .Response()
                .WithToken(token);
        }


        #region Private Methods
        private async Task<Domain.Entities.User> GetUser(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (user == null || !_userPasswordDomainService.VerifyPassword(user, request.Password))
                throw new InputValidationFailedApplicationException("Invalid username or password.");
            return user;
        }
        #endregion
    }
}
