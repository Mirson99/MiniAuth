using MediatR;
using MiniAuth.Application.Responses;

namespace MiniAuth.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password): IRequest<LoginResponse>;