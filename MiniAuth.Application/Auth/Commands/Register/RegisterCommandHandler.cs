using MediatR;
using MiniAuth.Application.Common.Exceptions;
using MiniAuth.Application.Common.Interfaces;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Auth.Commands.Register;

public class RegisterCommandHandler: IRequestHandler<RegisterCommand, User>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    
    public RegisterCommandHandler(IPasswordHasher passwordHasher, IUserRepository userRepository)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new ValidationException("User already exists");
        }
        var user = new User
        {
            Email = request.Email.ToLower(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
        };

        return await _userRepository.CreateAsync(user, cancellationToken);
    }
}