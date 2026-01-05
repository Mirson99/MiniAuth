using MediatR;
using MiniAuth.Application.Responses;

namespace MiniAuth.Application.Auth.Commands.LoginWithRefreshToken;

public sealed record LoginWithRefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;